using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EnergoImport.Models
{
    public class DataImportManager
    {
        const string connectionStringUGES = "Server=tcp:92.50.142.178,11000; Database=REG2; User Id=Ramina; Password=HLR735;";
        const string connectionStringCES = "Server=tcp:92.50.142.178,11000; Database=REG3; User Id=Ramina; Password=HLR735;";

        public enum ContractType { Undefined, CES, UGES }
        public Dictionary<ContractType, string> ContractNames { get; }
        public Dictionary<ContractType, string> ConnectionStrings { get; }
        public Dictionary<ContractType, string> RegPointsSqlStrings { get; }

        Contract GetContract(ContractType contractType)
        {
            using (RegPointsContext context = new RegPointsContext())
            {
                string contractName = "";
                if (contractType == ContractType.CES) contractName = "ПО ЦЭС";
                if (contractType == ContractType.UGES) contractName = "ПО УГЭС";
                return context.Contracts.Where(c => c.Name == contractName).FirstOrDefault();
            }
        }

        public DataImportManager()
        {
            //Названия контрактов
            ContractNames = new Dictionary<ContractType, string>();
            ContractNames.Add(ContractType.Undefined, "");
            ContractNames.Add(ContractType.CES, "ПО ЦЭС");
            ContractNames.Add(ContractType.UGES, "ПО УГЭС");
            //Строки подключения для старых баз
            ConnectionStrings = new Dictionary<ContractType, string>();
            ConnectionStrings.Add(ContractType.Undefined, "");
            ConnectionStrings.Add(ContractType.CES, connectionStringCES);
            ConnectionStrings.Add(ContractType.UGES, connectionStringUGES);
            //Строки запроса точек учета
            RegPointsSqlStrings = new Dictionary<ContractType, string>();
            RegPointsSqlStrings.Add(ContractType.Undefined, "");
            string sqlRequest = "";
            string path = HostingEnvironment.MapPath("~/App_Data/SQLRequestRegPointsCES.txt");
            if (File.Exists(path))
            {
                sqlRequest = File.ReadAllText(path);
                RegPointsSqlStrings.Add(ContractType.CES, sqlRequest);
            }
            path = HostingEnvironment.MapPath("~/App_Data/SQLRequestRegPointsUGES.txt");
            if (File.Exists(path))
            {
                sqlRequest = File.ReadAllText(path);
                RegPointsSqlStrings.Add(ContractType.UGES, sqlRequest);
            }
        }
        
        NetRegion GetNetRegion(string netRegionName)
        {
            using (RegPointsContext context = new RegPointsContext())
            {
                return context.NetRegions.FirstOrDefault(c => c.Name == netRegionName);
            }
        }
        //Инициализировать базу (есл она еще не была создана)
        public void InitContractsAndRegions()
        {
            using (RegPointsContext db = new RegPointsContext())
            {
                //Пользователи
                //Пользователь Admin
                if (db.Users.FirstOrDefault(u => u.Name == "admin") == null)
                    db.Users.Add(new User() { Name = "Администратор",
                        Login = "admin", Pass = "e920am02vladimir",
                        AccessImport = true,
                        AccessComments = true,
                        EditStatusInEnergo = true,
                        EditStatusAdded = true,
                        AccessDbCES = true,
                        AccessDbUGES = true
                    });
                if (db.Users.FirstOrDefault(u => u.Name == "guest") == null)
                    db.Users.Add(new User() { Name = "Гость", Login = "guest", Pass = "1234",
                        AccessImport = false,
                        AccessComments = true,
                        EditStatusInEnergo = false,
                        EditStatusAdded = true });
                //Договора и районы
                if (db.Contracts.Count() == 0)
                {
                    
                    //Договора
                    db.Contracts.Add(new Contract() { Name = "ПО ЦЭС" });
                    db.Contracts.Add(new Contract() { Name = "ПО УГЭС" });
                    db.SaveChanges();

                    var uges = GetContract(ContractType.UGES);
                    if (uges != null)
                    {
                        List<NetRegion> regionsUGES = LoadNetRegions(ContractType.UGES);
                        regionsUGES.ForEach(item =>
                        {
                            item.ContractId = uges.Id;
                            db.NetRegions.Add(item);
                        });
                    }

                    var ces = GetContract(ContractType.CES);
                    if (ces != null)
                    {
                        List<NetRegion> regionsUGES = LoadNetRegions(ContractType.CES);
                        regionsUGES.ForEach(item =>
                        {
                            item.ContractId = ces.Id;
                            db.NetRegions.Add(item);
                        });
                    }
                    db.SaveChanges();
                }
            }
        }

        public List<NetRegion> LoadNetRegions(ContractType contractType)
        {
            string connectionString = "";
            if (contractType == ContractType.UGES) connectionString = connectionStringUGES;
            if (contractType == ContractType.CES) connectionString = connectionStringCES;

            List<NetRegion> regions = new List<NetRegion>();
            //Read
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID_net, Name_short, Name_long FROM Net");
                command.Connection = myConnection;
                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    regions.Add(new NetRegion() { Name = reader.GetString(1), LongName = reader.GetString(2) });
                }
                reader.Close();
            }
            //Data
            return regions;
        }

        public List<string> LoadESubstations(ContractType contractType, int userId)
        {
            List<string> changes = new List<string>();
            
            using (RegPointsContext db = new RegPointsContext())
            {
                //Если нет пользователя с таким Id то выходим
                if (db.Users.FirstOrDefault(u => u.Id == userId) == null) return changes;
                //Подключемся к старой базе
                using (SqlConnection myConnection = new SqlConnection(ConnectionStrings[contractType]))
                {
                    SqlCommand command = new SqlCommand(
                        "SELECT * FROM [dbo].Net_object, Net " +
                        "WHERE ID_net = Net_ID_FK " +
                        "ORDER BY Net_ID_FK ");

                    command.Connection = myConnection;
                    myConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    NetRegion region = null;
                    string regionName = "";
                    //Read
                    while (reader.Read())
                    {
                        if (regionName != (string)reader["Name_short"])
                        {
                            regionName = (string)reader["Name_short"];
                            region = db.NetRegions.FirstOrDefault(c => c.Name == regionName);
                        }
                        if (region != null)
                        {
                            //Создаем объект из данных старой базы
                            ESubstation eSubNew = new ESubstation()
                            {
                                Name = (string)reader["Name_object"],
                                OldId = (int)reader["ID_object"],
                                NetRegionId = region.Id
                            };
                            if (reader["Date"].ToString() != "") eSubNew.AddDate = (DateTime)reader["Date"];

                            //Проверяем есть ли в нашей базе объект в определенном Контракте (обязательная проверка - имена ТП/РП совпадают иногда)
                            string cName = ContractNames[contractType];
                            var eSubExsist = db.ESubstations.FirstOrDefault(o =>
                                o.OldId == eSubNew.OldId &&
                                o.NetRegion.Contract.Name == cName
                            );


                            if (eSubExsist != null)
                            {
                                //Если есть, то проверим не изменилось ли у него имя
                                if (eSubNew.Name != eSubExsist.Name)
                                {
                                    changes.Add(eSubExsist.Name + " переименован в " + eSubNew.Name);
                                    //Если изменилось имя, то Добавим событие
                                    eSubExsist.Actions.Add(new Models.Action() { ActionType = ActionType.Rename, Comment = eSubExsist.Name + " переименован в " + eSubNew.Name, Time = DateTime.Now, UserId = userId });
                                    //Меняем имя и в нашей базе
                                    eSubExsist.Name = eSubNew.Name;
                                }
                                //Если изменился район в котором находится подстанция
                                if (eSubNew.NetRegionId != eSubExsist.NetRegionId)
                                {
                                    changes.Add(eSubExsist.Name + " изменен район с "+ eSubExsist.NetRegion.Name + " на " + regionName);
                                    //Если изменилось имя, то Добавим событие
                                    eSubExsist.Actions.Add(new Models.Action() { ActionType = ActionType.Rename, Comment = eSubExsist.Name + " изменен район на " + regionName, Time = DateTime.Now, UserId = userId });
                                    //Меняем имя и в нашей базе
                                    eSubExsist.NetRegionId = eSubNew.NetRegionId;
                                }
                            }
                            else
                            {
                                changes.Add("Добавлен: " + eSubNew.Name);
                                //Если объекта нет в базе, то добавляем его
                                eSubNew.Actions.Add(new Models.Action() { ActionType = ActionType.Import, Time = DateTime.Now, UserId = userId });
                                db.ESubstations.Add(eSubNew);
                            }
                        }
                        //Сохраним изменения в базе
                        db.SaveChanges();
                    }
                    reader.Close();
                }
            }
            //Вернем отчет
            return changes;
        }

        public List<RegPoint> LoadRegPoints(string netRegionName, int userId)
        {
            List<RegPoint> regPoints = new List<RegPoint>();
            
            //Получим строку подключения к базе
            string connectionString = string.Empty;
            string sqlRequest = string.Empty;
            //Подготовим строку подкючения и sql запрос из файла
            using (RegPointsContext db = new RegPointsContext())
            {
                //Если нет пользователя с таким Id то выходим
                if (db.Users.FirstOrDefault(u => u.Id == userId) == null) return regPoints;

                var region = db.NetRegions.FirstOrDefault(r => r.Name == netRegionName);
                if (region == null) return regPoints;
                ContractType contractType = ContractType.Undefined;
                if (region.Contract.Name == "ПО ЦЭС") contractType = ContractType.CES;
                if (region.Contract.Name == "ПО УГЭС") contractType = ContractType.UGES;
                //Получим строку подключения и запрос на точки
                connectionString = ConnectionStrings[contractType];
                sqlRequest = RegPointsSqlStrings[contractType];
                //Если что-то не ок, то возвращаем пустой список
                if (connectionString == string.Empty || sqlRequest == string.Empty || contractType == ContractType.Undefined) return regPoints;

                //Импортируем данные
                using (SqlConnection myConnection = new SqlConnection(connectionString))
                {
                    //Добавим условие в запрос для конкретного Района
                    sqlRequest += " WHERE [Net].[Name_short] = '" + netRegionName + "' ORDER BY Net_object.ID_object";
                    //Preparing to Get data
                    SqlCommand command = new SqlCommand(sqlRequest, myConnection);
                    myConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    string eSubName = "";
                    ESubstation eSub = null;
                    //Загрузим существующие точки в этом районе
                    List<RegPoint> existPoints = db.RegPoints.Where(p => p.ESubstation.NetRegion.Name == netRegionName).ToList();
                    while (reader.Read())
                    {
                        if (netRegionName != reader["Name_short"].ToString()) continue;
                        //Ищем объект в нашей базе
                        if(eSubName != reader["Name_object"].ToString())
                        {
                            eSubName = reader["Name_object"].ToString();
                            eSub = db.ESubstations.FirstOrDefault(e => e.Name == eSubName && e.NetRegion.Name == netRegionName);
                        }
                        //Если не нашли объект или счетчик не привязан к точке => то пропускаем
                        if (eSub == null || reader["Serial_number"].ToString() == "") continue;
                        
                        //Создаем точку учета из полученных данных
                        RegPoint rpNew = null;
                        if (contractType == ContractType.CES)
                        {
                            rpNew = new RegPoint()
                            {
                                OldId = (int)reader["ID_reg_point"],
                                Address =
                                reader["U_Street"].ToString() +
                                reader["U_House"].ToString() +
                                reader["U_Build"].ToString() +
                                reader["U_Flat"].ToString(),
                                Local = reader["U_Local"].ToString(),
                                FIO = reader["Name_consumer"].ToString(),
                                TypePU = reader["Name"].ToString(),
                                TypeLink = reader["Description"].ToString(),
                                Serial = reader["Serial_number"].ToString(),
                                AcceptedInEnergo = false,
                                AddedInEnergo = false,
                                LinkIsOk = false,
                                PhoneNumber = reader["Phone_number"].ToString(),
                                InstallPlace = reader["Install_place"].ToString(),
                                TTKoefficient = reader["TT_coefficient_A"].ToString(),
                            };
                            if (reader["Add_date"].ToString() != "") rpNew.AddDate = (DateTime)reader["Add_date"];
                        }
                        if (contractType == ContractType.UGES)
                        {
                            rpNew = new RegPoint()
                            {
                                OldId = (int)reader["ID_reg_point"],
                                Address = reader["Adress"].ToString(),
                                FIO = reader["Name_consumer"].ToString(),
                                TypePU = reader["Name"].ToString(),
                                TypeLink = reader["Description"].ToString(),
                                Serial = reader["Serial_number"].ToString(),
                                AcceptedInEnergo = false,
                                AddedInEnergo = false,
                                LinkIsOk = false,
                                PhoneNumber = reader["Phone_number"].ToString(),
                                InstallPlace = reader["Install_place"].ToString(),
                                TTKoefficient = reader["TT_coefficient_A"].ToString(),
                            };
                            if (reader["Add_date"].ToString() != "") rpNew.AddDate = (DateTime)reader["Add_date"];
                        }

                        //Проверим - есть ли эта точка на этой подстанции
                        RegPoint rpExsist = eSub.RegPoints.FirstOrDefault(p => p.Serial == rpNew.Serial); //Смотрим по серийнику
                        if(rpExsist != null)
                        {
                            bool upd = false;
                            string comment = "";
                            //Проверим изменились ли данные
                            if (rpExsist.Address != rpNew.Address){ rpExsist.Address = rpNew.Address; upd = true; comment += rpNew.Address; }
                            if (rpExsist.FIO != rpNew.FIO) { rpExsist.FIO = rpNew.FIO; upd = true; comment += rpNew.FIO; }
                            if (rpExsist.InstallPlace != rpNew.InstallPlace) { rpExsist.InstallPlace = rpNew.InstallPlace; upd = true; comment += rpNew.InstallPlace; }
                            if (rpExsist.PhoneNumber != rpNew.PhoneNumber) { rpExsist.PhoneNumber = rpNew.PhoneNumber; upd = true; comment += rpNew.PhoneNumber; }
                            if (rpExsist.TTKoefficient != rpNew.TTKoefficient) { rpExsist.TTKoefficient = rpNew.TTKoefficient; upd = true; comment += rpNew.TTKoefficient; }
                            if (rpExsist.TypeLink != rpNew.TypeLink) { rpExsist.TypeLink = rpNew.TypeLink; upd = true; comment += rpNew.TypeLink; }
                            if (upd)
                                rpExsist.Actions.Add(new Action() {
                                    ActionType = ActionType.RegPointUpdate,
                                    ESubstationId = eSub.Id,
                                    Time = DateTime.Now,
                                    UserId = userId,
                                    Comment = comment
                                });
                            db.SaveChanges();
                        }
                        else//Если нет то добавляем
                        {
                            //Действие
                            rpNew.Actions.Add(new Action() { ActionType = ActionType.RegPointAdd, ESubstation = eSub, Time = DateTime.Now, UserId = userId });
                            //Добавим точку в базу
                            eSub.RegPoints.Add(rpNew);
                            //Добавим в список для отчета
                            regPoints.Add(rpNew);
                            db.SaveChanges();
                        }
                    }
                    reader.Close();
                }
                //Сохраним
                db.SaveChanges();
            }
            
            return regPoints;
        }

        public void UpdateAllPoints(int userId)
        {
            //Объекты
            LoadESubstations(ContractType.CES, userId);
            LoadESubstations(ContractType.UGES, userId);
            //Точки
            using (RegPointsContext db = new RegPointsContext())
            {
                foreach(var reg in db.NetRegions) LoadRegPoints(reg.Name, userId);
            }
        }

        public Contract LoadAllPointsFromContractDb(ContractType contractType)
        {
            if (contractType == ContractType.Undefined) return null;
            Contract contract = new Contract() { Name = ContractNames[contractType] };
            //Load regions to contract
            LoadNetRegions(contractType).ForEach(item => contract.NetRegions.Add(item));

            //Загрузим нужный SQL запрос
            string sqlRequest = RegPointsSqlStrings[contractType];
            string connectionString = ConnectionStrings[contractType];
            if (connectionString == string.Empty || sqlRequest == string.Empty) return null;

            //Импортируем данные
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                //Preparing to Get data
                SqlCommand command = new SqlCommand(sqlRequest + /*" WHERE [Net].[Name_short] = '" + n.Name + "' " + */" ORDER BY Net_object.ID_object", myConnection);
                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                string eSubName = "";
                int eSubOldId = 0;
                ESubstation eSub = null;
                string netRegionName = "";
                NetRegion netRegion = null;
                while (reader.Read())
                {
                    if(netRegionName != reader["Name_short"].ToString())
                    {
                        netRegionName = reader["Name_short"].ToString();
                        netRegion = contract.NetRegions.FirstOrDefault(n => n.Name == netRegionName);
                        if (netRegion == null) continue;
                    }
                    //Ищем объект
                    if(reader["Name_object"].ToString() != "" && reader["ID_object"].ToString() != "")
                        if (eSubName != reader["Name_object"].ToString() || eSubOldId != (int)reader["ID_object"])
                        {
                            eSubName = reader["Name_object"].ToString();
                            eSubOldId = (int)reader["ID_object"];
                            eSub = netRegion.ESubstatons.FirstOrDefault(e => e.Name == eSubName && e.OldId == eSubOldId);
                            if (eSubName == "") continue;
                            if(eSub == null)
                            {
                                eSub = new ESubstation()
                                {
                                    Name = eSubName,
                                    OldId = eSubOldId
                                };
                                if (reader["Add_date"].ToString() != "") eSub.AddDate = (DateTime)reader["Add_date"];
                                netRegion.ESubstatons.Add(eSub);
                            }
                        }
                    //Если счетчик не привязан к точке => пропускаем
                    if (reader["Serial_number"].ToString() == "") continue;

                    //Создаем точку учета из полученных данных
                    RegPoint rpNew = null;
                    if (contractType == ContractType.CES)
                    {
                        rpNew = new RegPoint()
                        {
                            OldId = (int)reader["ID_reg_point"],
                            Address =
                            reader["U_Street"].ToString() +
                            reader["U_House"].ToString() +
                            reader["U_Build"].ToString() +
                            reader["U_Flat"].ToString(),
                            Local = reader["U_Local"].ToString(),
                            FIO = reader["Name_consumer"].ToString(),
                            TypePU = reader["Name"].ToString(),
                            TypeLink = reader["Description"].ToString(),
                            Serial = reader["Serial_number"].ToString(),
                            AcceptedInEnergo = false,
                            AddedInEnergo = false,
                            LinkIsOk = false,
                            PhoneNumber = reader["Phone_number"].ToString(),
                            InstallPlace = reader["Install_place"].ToString(),
                            TTKoefficient = reader["TT_coefficient_A"].ToString()
                        };
                        if (reader["Add_date"].ToString() != "") rpNew.AddDate = (DateTime)reader["Add_date"];
                    }
                    if (contractType == ContractType.UGES)
                    {
                        rpNew = new RegPoint()
                        {
                            OldId = (int)reader["ID_reg_point"],
                            Address = reader["Adress"].ToString(),
                            FIO = reader["Name_consumer"].ToString(),
                            TypePU = reader["Name"].ToString(),
                            TypeLink = reader["Description"].ToString(),
                            Serial = reader["Serial_number"].ToString(),
                            AcceptedInEnergo = false,
                            AddedInEnergo = false,
                            LinkIsOk = false,
                            PhoneNumber = reader["Phone_number"].ToString(),
                            InstallPlace = reader["Install_place"].ToString(),
                            TTKoefficient = reader["TT_coefficient_A"].ToString()
                        };
                        if (reader["Add_date"].ToString() != "") rpNew.AddDate = (DateTime)reader["Add_date"];
                    }

                    //Добавим точку в подстанцию
                    eSub.RegPoints.Add(rpNew);
                }
                reader.Close();
            }
            
            return contract;
        }
    }
}