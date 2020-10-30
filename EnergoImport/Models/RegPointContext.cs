namespace EnergoImport.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class RegPointsContext : DbContext
    {
        // Your context has been configured to use a 'RegPointContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'EnergoImport.Models.RegPointContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'RegPointContext' 
        // connection string in the application configuration file.
        public RegPointsContext()
            : base("name=RegPointsContext")
        {
        }
        
        public DbSet<Action> Actions { get; set; }  //Новости 

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<RegPoint> RegPoints { get; set; }      //Точки учета
        public DbSet<ESubstation> ESubstations { get; set; } //Объекты (подстанции)
        public DbSet<StatusESub> StatusesESub { get; set; } //Объекты (подстанции)
        public DbSet<NetRegion> NetRegions { get; set; }    //Районы
        public DbSet<Contract> Contracts { get; set; }      //Договора
    }
    //Какое-то действие с подстанцией или точкой
    public enum ActionType : int {
        Import,                 //Подстанция добавлена в базу
        Rename,                 //Подстанция переименована
        Delete,                 //Подстанция удалена
        RegPointAdd,            //Точка учета добавлена
        RegPointUpdate,         //Точка учета изменена
        RegPointDelete,         //Точка учета удалена
        CheckedInEnergosphera,  //Точка учета отмечена как "Принято в Энергосферу"
        UncheckedInEnergosphera,//Убрана отметка "Принято в Энергосферу"
        CheckedLinkIsOk,        //Точка учета отмечена как "Опрошена в УСПД"
        UncheckedLinkIsOk,      //Убрана отметка "Опрошена в УСПД"
        CheckedAdd,             //Точка учета отмечена как "Добавлена в Энергосферу"
        UncheckedAdd,           //Убрана отметка "Добавлена в Энергосферу"
        AddComment,             //Добавил комментарий
        DeleteComment,          //Удалил комментарий
        ChangeESubStatus,       //Изменил статус подстанции
        CheckedInSmeta,         //Точка учета отмечена как "Добавлена в Смету"
        UncheckedInSmeta,       //Убрана отметка "Добавлена в Смету"
        CheckedPollRequest,     //Точка учета отмечена как "Добавлена в Смету"
        UncheckedPollRequest    //Убрана отметка "Добавлена в Смету"
        //
        //!!! НЕЛЬЗЯ МЕНЯТЬ ПОРЯДОК СТРОК, МОЖНО ТОЛЬКО ДОБАВИТЬ НОВЫЕ
    }
    public class Action
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public ActionType ActionType { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; }
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int UserId { get; set; } //Код пользователя FK
        public virtual User User { get; set; }

        public int? RegPointId { get; set; }
        public virtual RegPoint RegPoint { get; set; }

        public int ESubstationId { get; set; } //Код подстанции FK
        public virtual ESubstation ESubstation { get; set; }
    }
    //Комментарии
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; }
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int UserId { get; set; } //Код пользователя FK
        public virtual User User { get; set; }

        [Required]
        public int ESubstationId { get; set; } //Код подстанции FK
        public virtual ESubstation ESubstation { get; set; }
    }
    //Пользователи
    public class User
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Имя")]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Логин")]
        [MaxLength(50)]
        public string Login { get; set; }
        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string Pass { get; set; }

        [DefaultValue("01.01.2000")]
        [DisplayName("Последний просмотр новостей")]
        public DateTime LastNewsViewTime { get; set; }

        //Разрешения
        [Required]
        [DefaultValue(false)]
        [DisplayName("Админка")]
        public bool EditUsers { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("БД УГЭС")]
        public bool AccessDbUGES { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("БД ЦЭС")]
        public bool AccessDbCES { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Импорт")]
        public bool AccessImport { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Изм. 'Принято в ЭС'")]
        public bool EditStatusInEnergo { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Изм. 'Связь'")]
        public bool EditStatusLinkIsOk { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Изм. 'Добавлено'")]
        public bool EditStatusAdded { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Коментарии")]
        public bool AccessComments { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Изм. статус ТП/РП")]
        public bool EditESubStatus { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("Удалить точку учета")]
        public bool DeleteRegPoint { get; set; }
    }

    //Точки учета
    public class RegPoint 
    {
        [Key]
        public int Id { get; set; }
        public int OldId { get; set; } //Id из старой базы (на случай изменения имени подстанции)
        [DefaultValue("01.01.2000")]
        public DateTime AddDate { get; set; } //Дата добавления в старой базе
        [Display(Name = "Населенный пункт")]
        [MaxLength(100)]
        public string Local { get; set; }
        [Display(Name = "Адрес")]
        [MaxLength(1000)]
        public string Address { get; set; }
        [Display(Name = "ФИО")]
        [MaxLength(1000)]
        public string FIO { get; set; }
        [Display(Name = "Тип ПУ")]
        [MaxLength(50)]
        public string TypePU { get; set; }
        [Display(Name = "Тип связи")]
        [MaxLength(50)]
        public string TypeLink { get; set; }
        [Display(Name = "Серийный номер")]
        [MaxLength(30)]
        public string Serial { get; set; }
        [Display(Name = "Место установки")]
        [MaxLength(100)]
        public string InstallPlace { get; set; }
        [Display(Name = "Номер тел.")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [Display(Name = "ТТ")]
        [MaxLength(20)]
        public string TTKoefficient { get; set; }
        //Flags status
        public bool AcceptedInEnergo { get; set; }
        public bool AddedInEnergo { get; set; }
        public bool LinkIsOk { get; set; }
        public bool InSmeta { get; set; }

        [Required]
        public int ESubstationId { get; set; } //Код подстанции FK
        public virtual ESubstation ESubstation { get; set; }

        public virtual List<Action> Actions { get; set; }

        public RegPoint()
        {
            Actions = new List<Action>();
        }
    }
    //Статусы подстанции
    public class StatusESub
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public virtual List<ESubstation> ESubstations { get; set; }
        
        public StatusESub()
        {
            ESubstations = new List<ESubstation>();
        }
    }
    //Объекты (ТП, РП)
    public class ESubstation
    {
        [Key]
        public int Id { get; set; }
        public int OldId { get; set; } //Id из старой базы (на случай изменения имени подстанции)
        [DefaultValue("01.01.2000")]
        public DateTime AddDate { get; set; } //Дата добавления в старой базе
        [MaxLength(50)]
        public string Name { get; set; }
        //Флаг - Нужно опросить
        [DefaultValue(false)]
        public bool PollRequest { get; set; }

        [Required]
        public int NetRegionId { get; set; }  //Код района (сетей) FK
        public virtual NetRegion NetRegion { get; set; }

        [Required][DefaultValue(1)]
        public int StatusESubId { get; set; } //Код статуса FK
        public virtual StatusESub StatusESub { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<RegPoint> RegPoints { get; set; }
        public virtual List<Action> Actions { get; set; }

        public ESubstation()
        {
            StatusESubId = 1;
            Comments = new List<Comment>();
            RegPoints = new List<RegPoint>();
            Actions = new List<Action>();
        }
    }
    //Районы (УРЭС, ВРЭС и тд)
    public class NetRegion
    {
        public int Id { get; set; }
        //Названия
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string LongName { get; set; }
        
        [Required]
        public int ContractId { get; set; } //Код договора FK
        public virtual Contract Contract { get; set; }

        public virtual List<ESubstation> ESubstatons { get; set; }

        public NetRegion()
        {
            ESubstatons = new List<ESubstation>();
        }

    }
    //Договора (ПО ЦЭС, ПО УГЭС)
    public class Contract
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }

        public virtual List<NetRegion> NetRegions { get; set; }

        public Contract()
        {
            NetRegions = new List<NetRegion>();
        }
    }

}