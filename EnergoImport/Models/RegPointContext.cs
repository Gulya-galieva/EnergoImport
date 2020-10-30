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
        
        public DbSet<Action> Actions { get; set; }  //������� 

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<RegPoint> RegPoints { get; set; }      //����� �����
        public DbSet<ESubstation> ESubstations { get; set; } //������� (����������)
        public DbSet<StatusESub> StatusesESub { get; set; } //������� (����������)
        public DbSet<NetRegion> NetRegions { get; set; }    //������
        public DbSet<Contract> Contracts { get; set; }      //��������
    }
    //�����-�� �������� � ����������� ��� ������
    public enum ActionType : int {
        Import,                 //���������� ��������� � ����
        Rename,                 //���������� �������������
        Delete,                 //���������� �������
        RegPointAdd,            //����� ����� ���������
        RegPointUpdate,         //����� ����� ��������
        RegPointDelete,         //����� ����� �������
        CheckedInEnergosphera,  //����� ����� �������� ��� "������� � �����������"
        UncheckedInEnergosphera,//������ ������� "������� � �����������"
        CheckedLinkIsOk,        //����� ����� �������� ��� "�������� � ����"
        UncheckedLinkIsOk,      //������ ������� "�������� � ����"
        CheckedAdd,             //����� ����� �������� ��� "��������� � �����������"
        UncheckedAdd,           //������ ������� "��������� � �����������"
        AddComment,             //������� �����������
        DeleteComment,          //������ �����������
        ChangeESubStatus,       //������� ������ ����������
        CheckedInSmeta,         //����� ����� �������� ��� "��������� � �����"
        UncheckedInSmeta,       //������ ������� "��������� � �����"
        CheckedPollRequest,     //����� ����� �������� ��� "��������� � �����"
        UncheckedPollRequest    //������ ������� "��������� � �����"
        //
        //!!! ������ ������ ������� �����, ����� ������ �������� �����
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
        public int UserId { get; set; } //��� ������������ FK
        public virtual User User { get; set; }

        public int? RegPointId { get; set; }
        public virtual RegPoint RegPoint { get; set; }

        public int ESubstationId { get; set; } //��� ���������� FK
        public virtual ESubstation ESubstation { get; set; }
    }
    //�����������
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; }
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int UserId { get; set; } //��� ������������ FK
        public virtual User User { get; set; }

        [Required]
        public int ESubstationId { get; set; } //��� ���������� FK
        public virtual ESubstation ESubstation { get; set; }
    }
    //������������
    public class User
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("���")]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [DisplayName("�����")]
        [MaxLength(50)]
        public string Login { get; set; }
        [Required]
        [DisplayName("������")]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string Pass { get; set; }

        [DefaultValue("01.01.2000")]
        [DisplayName("��������� �������� ��������")]
        public DateTime LastNewsViewTime { get; set; }

        //����������
        [Required]
        [DefaultValue(false)]
        [DisplayName("�������")]
        public bool EditUsers { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("�� ����")]
        public bool AccessDbUGES { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("�� ���")]
        public bool AccessDbCES { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("������")]
        public bool AccessImport { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("���. '������� � ��'")]
        public bool EditStatusInEnergo { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("���. '�����'")]
        public bool EditStatusLinkIsOk { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("���. '���������'")]
        public bool EditStatusAdded { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("����������")]
        public bool AccessComments { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("���. ������ ��/��")]
        public bool EditESubStatus { get; set; }
        [Required]
        [DefaultValue(false)]
        [DisplayName("������� ����� �����")]
        public bool DeleteRegPoint { get; set; }
    }

    //����� �����
    public class RegPoint 
    {
        [Key]
        public int Id { get; set; }
        public int OldId { get; set; } //Id �� ������ ���� (�� ������ ��������� ����� ����������)
        [DefaultValue("01.01.2000")]
        public DateTime AddDate { get; set; } //���� ���������� � ������ ����
        [Display(Name = "���������� �����")]
        [MaxLength(100)]
        public string Local { get; set; }
        [Display(Name = "�����")]
        [MaxLength(1000)]
        public string Address { get; set; }
        [Display(Name = "���")]
        [MaxLength(1000)]
        public string FIO { get; set; }
        [Display(Name = "��� ��")]
        [MaxLength(50)]
        public string TypePU { get; set; }
        [Display(Name = "��� �����")]
        [MaxLength(50)]
        public string TypeLink { get; set; }
        [Display(Name = "�������� �����")]
        [MaxLength(30)]
        public string Serial { get; set; }
        [Display(Name = "����� ���������")]
        [MaxLength(100)]
        public string InstallPlace { get; set; }
        [Display(Name = "����� ���.")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [Display(Name = "��")]
        [MaxLength(20)]
        public string TTKoefficient { get; set; }
        //Flags status
        public bool AcceptedInEnergo { get; set; }
        public bool AddedInEnergo { get; set; }
        public bool LinkIsOk { get; set; }
        public bool InSmeta { get; set; }

        [Required]
        public int ESubstationId { get; set; } //��� ���������� FK
        public virtual ESubstation ESubstation { get; set; }

        public virtual List<Action> Actions { get; set; }

        public RegPoint()
        {
            Actions = new List<Action>();
        }
    }
    //������� ����������
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
    //������� (��, ��)
    public class ESubstation
    {
        [Key]
        public int Id { get; set; }
        public int OldId { get; set; } //Id �� ������ ���� (�� ������ ��������� ����� ����������)
        [DefaultValue("01.01.2000")]
        public DateTime AddDate { get; set; } //���� ���������� � ������ ����
        [MaxLength(50)]
        public string Name { get; set; }
        //���� - ����� ��������
        [DefaultValue(false)]
        public bool PollRequest { get; set; }

        [Required]
        public int NetRegionId { get; set; }  //��� ������ (�����) FK
        public virtual NetRegion NetRegion { get; set; }

        [Required][DefaultValue(1)]
        public int StatusESubId { get; set; } //��� ������� FK
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
    //������ (����, ���� � ��)
    public class NetRegion
    {
        public int Id { get; set; }
        //��������
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string LongName { get; set; }
        
        [Required]
        public int ContractId { get; set; } //��� �������� FK
        public virtual Contract Contract { get; set; }

        public virtual List<ESubstation> ESubstatons { get; set; }

        public NetRegion()
        {
            ESubstatons = new List<ESubstation>();
        }

    }
    //�������� (�� ���, �� ����)
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