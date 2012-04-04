namespace FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Edm.Db;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Data.Entity.ModelConfiguration.Edm.Db.Mapping;
    using System.Data.Entity.ModelConfiguration.Edm.Services;
    using System.Data.Entity.Resources;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Windows.Media;
    using FunctionalTests.Model;
    using Xunit;

    #region Fixtures

    public class Party
    {
        public int Id { get; set; }
    }

    public class DayRecord
    {
        public int Id { get; set; }
        public Party ReportedBy { get; set; }
        public ICollection<HourlyForecast> Forecasts { get; set; }
    }

    public class HourlyForecast
    {
        public int Id { get; set; }
    }

    public class TypeClass
    {
        public int Id { get; set; }
        public int IntProp { get; set; }
        public string StringProp { get; set; }
        public byte[] ByteArrayProp { get; set; }
        public DateTime DateTimeProp { get; set; }
        public decimal DecimalProp { get; set; }
    }

    public abstract class AbsAtBase
    {
        public int Id { get; set; }
        public int Data { get; set; }
    }

    public class AbsAtBaseL1 : AbsAtBase
    {
        public int L1Data { get; set; }
    }

    public class AbsAtBaseL2 : AbsAtBaseL1
    {
        public int L2Data { get; set; }
    }

    public class AbsInMiddle
    {
        public int Id { get; set; }
        public int Data { get; set; }
    }

    public abstract class AbsInMiddleL1 : AbsInMiddle
    {
        public int L1Data { get; set; }
    }

    public class AbsInMiddleL2 : AbsInMiddleL1
    {
        public int L2Data { get; set; }
    }

    public class AssocBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseData { get; set; }
        public int AssocRelatedBaseId { get; set; }
        public AssocRelated AssocRelatedBase { get; set; }
    }

    public class AssocDerived : AssocBase
    {
        public int AssocRelatedId { get; set; }
        public string DerivedData1 { get; set; }
        public string DerivedData2 { get; set; }
        public AssocRelated AssocRelated { get; set; }
    }

    public class AssocRelated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AssocDerived> Deriveds { get; set; }
        public ICollection<AssocBase> Bases { get; set; }
        public AssocDerived RefDerived { get; set; }
        public AssocBase RefBase { get; set; }
    }

    public class AssocPrincipal
    {
        public int Id { get; set; }
        public AssocDependent DependentNavigation { get; set; }
    }

    public class AssocDependent : AssocBaseDependent
    {
        public AssocPrincipal PrincipalNavigation { get; set; }
    }

    public class AssocBaseDependent
    {
        public string BaseProperty { get; set; }
        public int Id { get; set; }
    }

    public struct FancyId
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
    }

    public class Stimulus
    {
        public int Id { get; set; }
        public Brush Background { get; set; }
    }

    public class TSItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TSItemDetail TSItemDetail { get; set; }
        public TSItemDetailDiffKey TSItemDetailDiffKey { get; set; }
        public TSItemDetailOverlappingProperty TSItemDetailOverlappingProperty { get; set; }
    }

    public class TSItemDetail
    {
        public int Id { get; set; }
        public string Detail { get; set; }
    }

    public class TSItemDetailDiffKey
    {
        public int DiffId { get; set; }
        public string Detail { get; set; }
    }

    public class TSItemDetailOverlappingProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TSIAItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TSIAItemDetail Detail { get; set; }
    }

    public class TSIAItemDetail
    {
        public string Id { get; set; }
        public string Detail { get; set; }
    }

    public class TSBase
    {
        public int Id { get; set; }
        public string BaseData { get; set; }
        public TSBaseDetail BaseDetail { get; set; }
    }

    public class TSBaseDetail
    {
        public int Id { get; set; }
        public string BaseDetail { get; set; }
    }

    public class TSDerived : TSBase
    {
        public string DerivedData { get; set; }
        public TSDerivedDetail Detail { get; set; }
    }

    public class TSDerivedDetail
    {
        public int Id { get; set; }
        public string DerivedDetail { get; set; }
    }

    public class CTBase
    {
        public int Id { get; set; }
        public CTAddress HomeAddress { get; set; }
        public CTAddress WorkAddress { get; set; }
    }

    public class CTAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public CTRegion Region { get; set; }
    }

    public class CTRegion
    {
        public string Country { get; set; }
        public string Zip { get; set; }
    }

    public class TPHBase
    {
        public int Id { get; set; }
        public string BaseData { get; set; }
        public int IntProp { get; set; }
        public int? NullableIntProp { get; set; }
    }

    public class TPHDerived : TPHBase
    {
        public string DerivedData { get; set; }
    }

    public class TPHLeaf : TPHDerived
    {
        public string LeafData { get; set; }
    }

    public class TPHRoot
    {
        public int Id { get; set; }
        public int RootData { get; set; }
    }

    public class TPHNodeA : TPHRoot
    {
        public int AData { get; set; }
    }

    public class TPHNodeB : TPHRoot
    {
        public int BData { get; set; }
    }

    public class TPHNodeC : TPHRoot
    {
        public int CData { get; set; }
    }

    // Hybrid tree:
    //       HybridBase
    //          /  \
    //  HybridL1A  HybridL1B
    //        /
    // HybridL2

    public class HybridBase
    {
        public int Id { get; set; }
        public int BaseData { get; set; }
    }

    public class HybridL1A : HybridBase
    {
        public int L1AData { get; set; }
    }

    public class HybridL1B : HybridBase
    {
        public int L1BData { get; set; }
    }

    public class HybridL2 : HybridL1A
    {
        public int L2Data { get; set; }
    }

    public class NullablePk
    {
        public int? Key { get; set; }
        public int? Id { get; set; }
    }

    public class AnnotatedNullablePk
    {
        [Key]
        public int? Key { get; set; }
    }

    public class BaseEntity
    {
        public long Id { get; set; }
        public string BaseClassProperty { get; set; }
        public virtual string VirtualBaseClassProperty { get; set; }
    }

    public class Unit : BaseEntity
    {
        public override string VirtualBaseClassProperty { get; set; }
    }

    public class ACrazy
    {
        public int Id { get; set; }
        public string A { get; set; }
    }

    public abstract class BCrazy : ACrazy
    {
        public string B { get; set; }
    }

    public class CCrazy : BCrazy
    {
        public string C { get; set; }
    }

    public abstract class DCrazy : CCrazy
    {
        public string D { get; set; }
    }

    public class ECrazy : DCrazy
    {
        public string E { get; set; }
    }

    public class XCrazy : DCrazy
    {
        public string X { get; set; }
    }

    public class IsolatedIsland
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CKCategory
    {
        public Guid Key1 { get; set; }
        public double? Key2 { get; set; }
    }

    public class CKImage
    {
        public Guid Key1 { get; set; }
        public double? Key2 { get; set; }
        public string ImageData { get; set; }
    }

    public class CKProduct
    {
        public Guid CKCategoryKey1 { get; set; }
        public double CKCategoryKey2 { get; set; }
        public CKCategory CKCategory { get; set; }
    }

    public class CKDerivedProduct : CKProduct
    {
        public decimal? DerivedProperty1 { get; set; }
        public CKImage Image { get; set; }
    }

    public class CKDerivedProduct2 : CKDerivedProduct
    {
        public decimal? DerivedProperty2 { get; set; }
    }

    public class SelfRefDerived : SelfRefBase
    {
        public float? DependentKey1 { get; set; }
        public SelfRefDerived SelfRefNavigation { get; set; }
    }

    public class SelfRefBase
    {
        public string BaseProperty { get; set; }
        public float Key1 { get; set; }
    }

    public class Repro136761_A
    {
        public int Repro136761_AId { get; set; }
        public string AData { get; set; }
    }

    public class Repro136761_B : Repro136761_A
    {
        public string BData { get; set; }
    }

    public class Repro136761_C
    {
        public int Repro136761_CId { get; set; }
        public string CData { get; set; }
        public int Repro136761_BId { get; set; }
        public Repro136761_B Repro136761_B { get; set; }
    }

    // Base association with property discriminator, composite nullable key types

    public class Repro136322_Principal
    {
        public byte[] Key1 { get; set; }
        public long? Key2 { get; set; }
    }

    public class Repro136322_Dependent
    {
        public byte[] PrincipalKey1 { get; set; }
        public long PrincipalKey2 { get; set; }
        public int Id { get; set; }
        public Repro136322_Principal PrincipalNavigation { get; set; }
    }

    public class Repro136322_DerivedDependent : Repro136322_Dependent
    {
        public string DerivedProperty1 { get; set; }
        public string Discriminator1 { get; set; }
    }

    // Base association, singleton non-nullable key type

    public class Repro136855_Dependent
    {
        public DateTime Key1 { get; set; }
        public int Id { get; set; }
        public Repro136855_Principal PrincipalNavigation { get; set; }
    }

    public class Repro136855_Principal
    {
        public DateTime Key1 { get; set; }
    }

    public class Repro136855_DerivedDependent : Repro136855_Dependent
    {
        public bool? DerivedProperty1 { get; set; }
    }

    // Derived association

    public class Repro135563_Dependent : Repro135563_BaseDependent
    {
        public Repro135563_Principal PrincipalNavigation { get; set; }
    }

    public class Repro135563_Principal
    {
        public decimal Key1 { get; set; }
        public short? Key2 { get; set; }
        public Repro135563_Dependent DependentNavigation { get; set; }
    }

    public class Repro135563_BaseDependent
    {
        public TimeSpan BaseProperty { get; set; }
        public int Id { get; set; }
    }

    // Three Level Hierarchy, abstract middle, self-ref in middle

    public class ThreeLevelBase
    {
        public string BaseProperty { get; set; }
        public float? Key1 { get; set; }
    }

    public abstract class ThreeLevelDerived : ThreeLevelBase
    {
        public ICollection<ThreeLevelDerived> DependentNavigation { get; set; }
    }

    public class ThreeLevelLeaf : ThreeLevelDerived
    {
        public string DerivedProperty1 { get; set; }
    }

    // Two level hierarchy with byte[] prop

    public class ByteBase
    {
        public int Id { get; set; }
        public byte[] ByteData { get; set; }
    }

    public class ByteDerived : ByteBase
    {
        public byte[] DerivedData { get; set; }
    }

    public class ByteDerived2 : ByteDerived
    {
        public byte[] ExtraData { get; set; }
    }

    // Association to abstract dependent base type

    public abstract class AbsDep_Dependent
    {
        public int Id { get; set; }
        public short PrincipalKey1 { get; set; }
        public AbsDep_Principal PrincipalNavigation { get; set; }
    }

    public class AbsDep_Principal
    {
        public short Key1 { get; set; }
        public ICollection<AbsDep_Dependent> DependentNavigation { get; set; }
    }

    public class AbsDep_DerivedDependent : AbsDep_Dependent
    {
        public byte[] DerivedProperty1 { get; set; }
    }

    // Association to a principal base type

    public class AssocBase_Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AssocBase_OldDepartment : AssocBase_Department
    {
        public DateTime ObsoleteDate { get; set; }
    }

    public class AssocBase_Employee
    {
        public int Id { get; set; }
        public int AssocBase_DepartmentId { get; set; }
        public AssocBase_Department AssocBase_Department { get; set; }
    }

    // Two Hierachies with association between subtypes

    public class THABS_BasePrincipal
    {
        public string BaseProperty { get; set; }
        public byte[] Key1 { get; set; }
    }

    public class THABS_Principal : THABS_BasePrincipal
    {
        public ICollection<THABS_Dependent> THABS_DependentNavigation { get; set; }
    }

    public class THABS_Dependent
    {
        public int Id { get; set; }
        public THABS_Principal THABS_PrincipalNavigation { get; set; }
    }

    public class THABS_DerivedDependent : THABS_Dependent
    {
        public DateTime DerivedProperty1 { get; set; }
    }

    // Self-ref on derived type

    public class SRBase
    {
        public string BaseProperty { get; set; }
        public float? Key1 { get; set; }
        public DateTime Key2 { get; set; }
    }

    public class SRDerived : SRBase
    {
        public ICollection<SRDerived> Navigation { get; set; }
    }

    // Table splitting on base...used with TPC

    public class Repro140106_Vehicle
    {
        public string Repro140106_VehicleId { get; set; }
        public string Name { get; set; }
        public Repro140106_Model Model { get; set; }
    }

    public class Repro140106_Car : Repro140106_Vehicle
    {
        public string Type { get; set; }
    }

    public class Repro140106_Model
    {
        public string Repro140106_ModelId { get; set; }
        public decimal Description { get; set; }
        public Repro140106_Vehicle Repro140106_Vehicle { get; set; }
    }

    // Table splitting on base...used with TPT

    public class Repro140107_Vehicle
    {
        public int? Repro140107_VehicleId { get; set; }
        public short? Name { get; set; }
        public Repro140107_Model Repro140107_Model { get; set; }
    }

    public class Repro140107_Car : Repro140107_Vehicle
    {
        public long Type { get; set; }
    }

    public class Repro140107_Model
    {
        public int? Repro140107_ModelId { get; set; }
        public bool Description { get; set; }
        public Repro140107_Vehicle Repro140107_Vehicle { get; set; }
    }

    // Hybrid mapping model

    public class Entity1
    {
        public short? Entity1_Col1 { get; set; }
        public DateTime Entity1_Col2 { get; set; }
        public byte[] Entity1_Col3 { get; set; }
        public string Id { get; set; }
    }

    public class Entity2 : Entity1
    {
        public DateTime? Entity2_Col1 { get; set; }
    }

    public class Entity3 : Entity1
    {
        public string Entity3_Col1 { get; set; }
        public DateTimeOffset Entity3_Col2 { get; set; }
    }

    public class Entity4 : Entity1
    {
        public long? Entity4_Col1 { get; set; }
        public string Entity4_Col2 { get; set; }
    }

    public class Entity5 : Entity2
    {
        public bool? Entity5_Col1 { get; set; }
        public DateTime Entity5_Col2 { get; set; }
        public TimeSpan Entity5_Col3 { get; set; }
        public decimal? Entity5_Col4 { get; set; }
        public DateTime Entity5_Col5 { get; set; }
    }

    // Hybrid Scenario

    public class Repro137329_A1
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Age1 { get; set; }
        public string Name { get; set; }
    }

    public class Repro137329_A2 : Repro137329_A1
    {
        public string Age2 { get; set; }
    }

    public class Repro137329_A3 : Repro137329_A2
    {
        public string Age3 { get; set; }
    }

    public class Repro137329_A4 : Repro137329_A1
    {
        public string Age4 { get; set; }
    }

    // Property Discriminator

    public class Repro143236_Dependent
    {
        public int Id { get; set; }
    }

    public class Repro143236_DerivedDependent : Repro143236_Dependent
    {
        public DateTime DerivedProperty1 { get; set; }
        public double DiscriminatorNotNull { get; set; }
    }

    // abstract base class

    public abstract class Repro143662_Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Repro143662_Agency : Repro143662_Party
    {
        public int Reputation { get; set; }
    }

    public class Repro143662_Person : Repro143662_Party
    {
        public string Address { get; set; }
    }

    // IA relationship between subtypes

    public class Repro109944_BaseEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }

    public class Repro109944_Entity1 : Repro109944_BaseEntity
    {
        public string SomeProperty { get; set; }
        public Repro109944_Entity2 Repro109944_Entity2 { get; set; }
        public int Repro109944_Entity2ID { get; set; }
    }

    public class Repro109944_Entity2 : Repro109944_BaseEntity
    {
        public string SomeProperty { get; set; }
    }

    // Base type to base type FK association

    public class Repro142961_Dependent
    {
        public decimal Key1 { get; set; }
        public int Id { get; set; }
        public Repro142961_Principal PrincipalNavigation { get; set; }
    }

    public class Repro142961_Principal
    {
        public decimal Key1 { get; set; }
    }

    public class Repro142961_DerivedDependent : Repro142961_Dependent
    {
        public Guid DerivedProperty1 { get; set; }
    }

    public class Repro142961_DerivedPrincipal : Repro142961_Principal
    {
        public long DerivedProperty1 { get; set; }
    }

    // Abstract base with discriminator

    public abstract class Repro142666_Dependent
    {
        public bool DependentForeignKeyPropertyNotFromConvention1 { get; set; }
        public int Id { get; set; }
    }

    public class Repro142666_DerivedDependent : Repro142666_Dependent
    {
        public string DerivedProperty1 { get; set; }
    }

    // Simple three level hierarchy

    public class Repro143127_EntityA
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Repro143127_EntityB : Repro143127_EntityA
    {
        public int Count { get; set; }
        public string Info { get; set; }
    }

    public class Repro143127_EntityC : Repro143127_EntityB
    {
        public int P1 { get; set; }
    }

    // IA from an abstract non-root type

    public abstract class Repro142682_Dependent : Repro142682_BaseDependent
    {
        public DateTime DependentForeignKeyPropertyNotFromConvention1 { get; set; }
        public Repro142682_Principal PrincipalNavigation { get; set; }
    }

    public class Repro142682_Principal
    {
        public DateTime? Key1 { get; set; }
        public ICollection<Repro142682_Dependent> DependentNavigation { get; set; }
    }

    public class Repro142682_BaseDependent
    {
        public long? BaseProperty { get; set; }
        public int Id { get; set; }
    }

    public class Repro142682_DerivedDependent : Repro142682_Dependent
    {
        public decimal? DerivedProperty1 { get; set; }
    }

    // Simple hierarchy for TPC property renaming

    public class Repro144459_Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Repro144459_DiscontinuedProduct : Repro144459_Product
    {
        public DateTime DiscontinuedOn { get; set; }
    }

    public class Repro144459_ClearanceProduct : Repro144459_Product
    {
        public decimal SalePrice { get; set; }
    }

    // Abstract base as principal of FK association

    public abstract class Repro110459_Customer
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<Repro110459_Order> Orders { get; set; }
    }

    public class Repro110459_Account : Repro110459_Customer
    {
        public virtual string AccountNumber { get; set; }
    }

    public class Repro110459_Contact : Repro110459_Customer
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }

    public class Repro110459_Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Repro110459_Customer BillToCustomer { get; set; }
        public Guid BillToCustomerId { get; set; }
    }

    // Multiple relationships at various points in hierarchy with TPT

    public class Repro109916_BaseEntity
    {
        [Key]
        public int ID { get; set; }

        public string Title { get; set; }

        public Repro109916_Collectible Parent { get; set; }
        public virtual ICollection<Repro109916_Collectible> Children { get; set; }
    }

    public class Repro109916_Collectible
    {
        public int ID { get; set; }
    }

    public class Repro109916_SomeEntity : Repro109916_BaseEntity
    {
        public byte Units { get; set; }

        public string Watermark { get; set; }
        public Repro109916_BaseEntity BaseEntity { get; set; }
    }

    public class Repro109916_SomeMore : Repro109916_BaseEntity
    {
        [Column("Watermark")]
        public string Watermark { get; set; }

        public string Denomination { get; set; }

        // These cause the problem:
        public Repro109916_Product Product { get; set; }
        public Repro109916_SomeEntity SomeEntity { get; set; }
    }

    public class Repro109916_Product
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
    }

    // Three level hierarchy with abstract middle

    public class Repro147822_EntityA
    {
        public int Id { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public abstract class Repro147822_EntityB : Repro147822_EntityA
    {
        public string Details { get; set; }
    }

    public class Repro147822_EntityC : Repro147822_EntityB
    {
        public string Color { get; set; }
    }

    // Two level hierarchy with abstract root type

    public abstract class Repro147906_EntityA1
    {
        public int Id { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class Repro147906_EntityA1_1 : Repro147906_EntityA1
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }

    public class Repro147906_EntityA1_2 : Repro147906_EntityA1
    {
        public string Property4 { get; set; }
        public string Property5 { get; set; }
        public string Property6 { get; set; }
    }

    // Complex Type on a base entity

    public class Repro148415_EntityC1
    {
        public int Id { get; set; }
        public Repro148415_ComplexTypeC1 ComplexProperty1 { get; set; }
        public float Property2 { get; set; }
    }

    public class Repro148415_EntityC1_1 : Repro148415_EntityC1
    {
        public float Property3 { get; set; }
        public float Property4 { get; set; }
    }

    public class Repro148415_ComplexTypeC1
    {
        public int P1 { get; set; }
        public float P2 { get; set; }
    }

    // Table splitting on base used for TPC

    public class TPCVehicle
    {
        public string TPCVehicleId { get; set; }
        public string Name { get; set; }
        public TPCModel TPCModel { get; set; }
    }

    public class TPCCar : TPCVehicle
    {
        public string Type { get; set; }
    }

    public class TPCModel
    {
        public string TPCModelId { get; set; }
        public decimal Description { get; set; }
        public TPCVehicle TPCVehicle { get; set; }
    }

    // Three level hierarchy for entity splitting

    public class Repro147929_EntityB1
    {
        public int Id { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class Repro147929_EntityB1_1 : Repro147929_EntityB1
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
    }

    public class Repro147929_EntityB1_1_1 : Repro147929_EntityB1_1
    {
        public string Property4 { get; set; }
        public string Property5 { get; set; }
        public string Property6 { get; set; }
    }

    // Three level hierarchy with association between abstract middle types

    public class Repro150248_BaseDependent
    {
        public System.Nullable<System.DateTime> BaseProperty { get; set; }
        public string Key1 { get; set; }
    }

    public abstract class Repro150248_Dependent : Repro150248_BaseDependent
    {
        public string PrincipalKey1 { get; set; }
        public Repro150248_Principal PrincipalNavigation { get; set; }
    }

    public class Repro150248_DerivedDependent : Repro150248_Dependent
    {
        public System.Nullable<decimal> DerivedProperty1 { get; set; }
    }

    public abstract class Repro150248_Principal : Repro150248_BaseDependent
    {
        public ICollection<Repro150248_Dependent> DependentNavigation { get; set; }
    }

    public class Repro150248_DerivedPrincipal : Repro150248_Principal
    {
        public string DerivedProperty1 { get; set; }
    }

    // 1:0..1 IA on derived dependent

    public class Repro150634_BaseDependent
    {
        public string BaseProperty { get; set; }
        public System.Nullable<System.Guid> Key1 { get; set; }
        public System.Nullable<System.Guid> Key2 { get; set; }
    }

    public class Repro150634_Dependent : Repro150634_BaseDependent
    {
        public System.Int32 DependentForeignKeyPropertyNotFromConvention1 { get; set; }
        public Repro150634_Principal PrincipalNavigation { get; set; }
    }

    public class Repro150634_DerivedDependent : Repro150634_Dependent
    {
        public System.Int32 ExtraProp { get; set; }
    }

    public class Repro150634_Principal
    {
        public int Id { get; set; }
        public Repro150634_Dependent DependentNavigation { get; set; }
    }

    // composite key with property ordering

    public class EntityWithCompositePK
    {
        [Column(Order = 1)]
        public string Key1 { get; set; }

        [Column(Order = 2)]
        public int Key2 { get; set; }

        [Column(Order = 3)]
        public byte[] Property1 { get; set; }

        [Column(Order = 4)]
        public string Property2 { get; set; }
    }

    // Hierarchy with abstract base and FK on base

    public abstract class Repro150248_Dependent_2
    {
        public string Key1 { get; set; }
        public Repro150248_Principal_2 PrincipalNavigation { get; set; }
    }

    public class Repro150248_Principal_2 : Repro150248_BasePrincipal_2
    {
        public Repro150248_Dependent_2 DependentNavigation { get; set; }
    }

    public class Repro150248_DerivedDependent_2 : Repro150248_Dependent_2
    {
        public System.Nullable<byte> DependentDerivedProperty1 { get; set; }
    }

    public class Repro150248_BasePrincipal_2
    {
        public string Key1 { get; set; }
    }

    // hierarchy with middle type given a table name

    public class Repro143351_A1
    {
        [DatabaseGeneratedAttribute(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string A1Col1 { get; set; }
        public byte[] A1Col2 { get; set; }
    }

    [TableAttribute("NewA2TableName")]
    public class Repro143351_A2 : Repro143351_A1
    {
        public byte[] A2Col1 { get; set; }
    }

    public class Repro143351_A3 : Repro143351_A2
    {
        public System.DateTime A3Col1 { get; set; }
    }

    public class Repro143351_A4 : Repro143351_A1
    {
        public long A4Col1 { get; set; }
    }

    // self reference from derived to abstract base

    public class Repro135563_2_Dependent : Repro135563_2_BaseDependent
    {
        public Repro135563_2_BaseDependent PrincipalNavigation { get; set; }
    }

    public abstract class Repro135563_2_BaseDependent
    {
        public Guid BaseProperty { get; set; }
        public short Key1 { get; set; }
        public ICollection<Repro135563_2_Dependent> DependentNavigation { get; set; }
    }

    public class Repro135563_2_Other : Repro135563_2_BaseDependent
    {
        public string OtherData { get; set; }
    }

    // self reference from derived to concrete base

    public class Repro135563_3_Dependent : Repro135563_3_BaseDependent
    {
        public Repro135563_3_BaseDependent PrincipalNavigation { get; set; }
    }

    public class Repro135563_3_BaseDependent
    {
        public Guid BaseProperty { get; set; }
        public short Key1 { get; set; }
        public ICollection<Repro135563_3_Dependent> DependentNavigation { get; set; }
    }

    // Basic Inheritance

    public class EntityL
    {
        public int Id { get; set; }
        public string Property1 { get; set; }
        public byte[] Property2 { get; set; }
    }

    public class EntityL_1 : EntityL
    {
        public string Property3 { get; set; }
        public byte[] Property4 { get; set; }
    }

    // abstract at base for splitting

    public abstract class AbsBaseSplit
    {
        public int Id { get; set; }
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
    }

    public class AbsDerivedSplit : AbsBaseSplit
    {
        public string Prop3 { get; set; }
    }

    // abstract in middle for splitting

    public class EntityN
    {
        public int Id { get; set; }
        public int Property1 { get; set; }
        public int Property1_1 { get; set; }
    }

    public abstract class EntityN_1 : EntityN
    {
        public int Property2 { get; set; }
        public int Property2_1 { get; set; }
    }

    public class EntityN_1_1 : EntityN_1
    {
        public int Property3_1 { get; set; }
        public int Property3 { get; set; }
    }

    public abstract class BaseNote
    {
        public virtual Guid Id { get; set; }
        public virtual string Description { get; set; }
    }

    public class NoteWithRelationship1 : BaseNote
    {
        public virtual TargetEmployee TargetEmployee { get; set; }
    }

    public class NoteWithRelationship2 : BaseNote
    {
        public virtual TargetEmployee TargetEmployee { get; set; }
    }

    public abstract class BaseEmployee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TargetEmployee : BaseEmployee
    {
        public DateTime? DateOfBirth { get; set; }
    }

    // Reuse complex type in derived classes

    [Table("Product")]
    public abstract class VehicleProduct
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    [Table("RedVehicle")]
    public abstract class RedVehicle : VehicleProduct
    {
        public VehicleType VehicleType { get; set; } //The same name of complex type
    }

    [Table("BlueVehicle")]
    public abstract class BlueVehicle : VehicleProduct
    {
        public VehicleType VehicleType { get; set; } //The same name of complex type
    }

    [Table("RedCar")]
    public class RedCar : RedVehicle
    {
    }

    [Table("BlueCar")]
    public class BlueCar : BlueVehicle
    {
    }

    [ComplexType]
    public class VehicleType
    {
        [Required]
        public string Model { get; set; }
    }

    #endregion

    public sealed class BasicMappingScenarioTests : TestBase
    {
        #region Misc

        [Fact]
        public void Has_max_length_should_work_when_no_max_length_convention()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Conventions.Remove<PropertyMaxLengthConvention>();
            modelBuilder.Entity<TypeClass>().HasKey(a => a.StringProp);
            modelBuilder.Entity<TypeClass>().Property(t => t.StringProp).HasMaxLength(42);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<TypeClass>(t => t.StringProp).FacetEqual(true, f => f.IsUnicode);
            databaseMapping.Assert<TypeClass>(t => t.StringProp).FacetEqual(false, f => f.IsFixedLength);
            databaseMapping.Assert<TypeClass>(t => t.StringProp).FacetEqual(42, f => f.MaxLength);
            databaseMapping.Assert<TypeClass>(t => t.StringProp).DbFacetEqual(42, f => f.MaxLength);
        }

        [Fact]
        public void Should_be_able_to_call_simple_has_key_twice()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<CTAddress>().HasKey(a => a.Street);
            modelBuilder.Entity<CTAddress>().HasKey(a => a.Street);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Should_be_able_to_ignore_base_class_property()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<Unit>().Ignore(u => u.BaseClassProperty);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();

            Assert.False(
                databaseMapping.Model.Namespaces.Single().EntityTypes.Single().Properties.Any(
                    p => p.Name == "BaseClassProperty"));
        }

        [Fact]
        public void Should_be_able_to_ignore_overriden_unmapped_base_class_property()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<Unit>().Ignore(u => u.VirtualBaseClassProperty);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();

            Assert.False(
                databaseMapping.Model.Namespaces.Single().EntityTypes.Single().Properties.Any(
                    p => p.Name == "VirtualBaseClassProperty"));
        }

        [Fact]
        public void Ignoring_overriden_mapped_base_class_property_throws()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<BaseEntity>();
            modelBuilder.Entity<Unit>().Ignore(u => u.VirtualBaseClassProperty);

            Assert.Equal(
                Strings.CannotIgnoreMappedBaseProperty("VirtualBaseClassProperty", "FunctionalTests.Unit",
                                                       "FunctionalTests.BaseEntity"),
                Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo))
                    .Message);
        }

        [Fact]
        public void Should_be_able_to_configure_nullable_ospace_key()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<NullablePk>().HasKey(n => n.Key);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Should_be_able_to_annotate_nullable_ospace_key()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<AnnotatedNullablePk>();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Should_be_able_to_discover_nullable_ospace_key()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<NullablePk>();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Explicitly_configured_entity_set_name_is_not_pluralized()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Customer>().HasEntitySetName("Customer");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal("Customer", databaseMapping.Model.Containers.Single().EntitySets.Single().Name);
        }

        [Fact]
        public void Explicitly_configured_entity_set_names_are_uniqued()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Customer>().HasEntitySetName("Foo");
            modelBuilder.Entity<Product>().HasEntitySetName("Foo");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal("Foo", databaseMapping.Model.Containers.Single().EntitySets.First().Name);
            Assert.Equal("Foo1", databaseMapping.Model.Containers.Single().EntitySets.Last().Name);
        }

        [Fact]
        public void Build_model_for_type_with_framework_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Stimulus>();

            // WPF classes invalid
            Assert.Throws<ModelValidationException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Build_model_for_type_with_internal_member_to_configure_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Product>();
            modelBuilder.Entity<TransactionHistory>()
                .HasKey(th => th.TransactionID);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.Model.Namespaces.Single().AssociationTypes.Count);
        }

        [Fact]
        public void Build_model_for_type_with_configured_internal_members()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TransactionHistory>()
                .HasKey(th => th.TransactionID);
            modelBuilder.Entity<TransactionHistory>()
                .HasRequired(th => th.Product);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo, typeof(Product));

            Assert.Equal(1, databaseMapping.Model.Namespaces.Single().AssociationTypes.Count);
        }

        [Fact]
        public void Build_model_for_a_single_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TransactionHistoryArchive>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_after_customizing_property_column_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Customer>().Property(c => c.AccountNumber).HasColumnName("acc_no");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_after_customizing_property_column_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Customer>().Property(c => c.AccountNumber).HasColumnType("NVARCHAR");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_for_a_single_type_with_a_nullable_key()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Location>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_for_a_single_type_with_a_composite_key()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<StoreContact>().HasKey(sc => new { sc.ContactID, sc.CustomerID });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_for_a_single_type_with_a_string_key()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Culture>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Build_model_for_default_tph_mapping()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Product>();
            modelBuilder.Entity<DiscontinuedProduct>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        #endregion

        #region ToTable Tests

        [Fact]
        public void ToTable_on_single_entity_changes_table_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().ToTable("tbl_sp");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<SalesPerson>("tbl_sp");
        }

        [Fact]
        public void ToTable_on_single_entity_with_association_changes_table_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().ToTable("tbl_sp");
            modelBuilder.Entity<Employee>().HasOptional(o => o.SalesPerson).WithRequired(e => e.Employee);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<SalesPerson>("tbl_sp");
        }

        #endregion

        #region ToTable Schema Tests

        [Fact]
        public void Build_model_with_table_schema_configured()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().ToTable("tbl_sp", "sales");
            modelBuilder.Entity<Customer>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "sales"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
        }

        [Fact]
        public void Build_model_with_dotted_table_name_configured()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().ToTable("sales.tbl_sp");
            modelBuilder.Entity<Customer>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "sales"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
        }

        [Fact]
        public void Build_model_with_dotted_table_name_and_dotted_schema_configured()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().ToTable("sales.A.B.tbl_sp");
            modelBuilder.Entity<Customer>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "sales.A.B"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
        }

        [Fact]
        public void Relationship_with_table_in_different_schema_works()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<DayRecord>();
            modelBuilder.Entity<Party>();
            modelBuilder.Entity<HourlyForecast>().Map(m => m.ToTable("HourlyForecasts", "dbx"));

            var databaseMapping = BuildMapping(modelBuilder);

            var mws = databaseMapping.ToMetadataWorkspace();

            var storeCollection = mws.GetItemCollection(DataSpace.SSpace);
            Assert.Equal(2,
                         (storeCollection.GetItem<EntityContainer>("CodeFirstDatabase")).BaseEntitySets.OfType
                             <AssociationSet>().Count());
        }

        #endregion

        #region Basic TPH Tests

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_strings()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b"); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("MyDisc")
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "c");
        }

        [Fact]
        public void ToTable_on_root_abstract_class_renames_TPH_hierarchy()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBase>().ToTable("Foo");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AbsAtBase>("Foo");
        }

        [Fact]
        // Regression test for Dev 11 bug 136256
        public void Requires_value_can_change_discriminator_column_name_and_use_strings_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a"); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

            modelBuilder.Entity<TPHDerived>().Map(mc => { mc.Requires("MyDisc").HasValue("b").HasColumnType("ntext"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("MyDisc")
                .DbFacetEqual(null, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "c");
        }

        [Fact]
        // Regression test for Dev 11 bug 142718
        public void Requires_value_can_change_discriminator_column_name_and_nullability()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a").IsOptional(); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b"); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("MyDisc")
                .DbEqual(true, f => f.IsNullable)
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "c");
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_nullability_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b").IsOptional(); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("MyDisc")
                .DbEqual(true, f => f.IsNullable)
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "c");
        }

        [Fact]
        public void Requires_throws_on_conflicting_nullability()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a").IsRequired(); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b").IsOptional(); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        // Regression test for Dev 11 bug 136596
        public void Requires_value_works_with_table_annotation()
        {
            using (var tphBaseConfiguration = new DynamicTypeDescriptionConfiguration<TPHBase>())
            {
                tphBaseConfiguration.TypeAttributes = new[] { new TableAttribute("MegaTPH") };
                var modelBuilder = new AdventureWorksModelBuilder();

                modelBuilder.Entity<TPHBase>()
                    .Map(mc => { mc.Requires("MyDisc").HasValue("a"); })
                    .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b"); })
                    .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue("c"); });

                var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

                Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
                databaseMapping.Assert<TPHBase>()
                    .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
                databaseMapping.Assert<TPHBase>("MegaTPH")
                    .Column("MyDisc")
                    .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
                databaseMapping.AssertMapping<TPHBase>("MegaTPH", false)
                    .HasNoPropertyConditions()
                    .HasColumnCondition("MyDisc", "a");
                databaseMapping.AssertMapping<TPHDerived>("MegaTPH")
                    .HasNoPropertyConditions()
                    .HasColumnCondition("MyDisc", "b");
                databaseMapping.AssertMapping<TPHLeaf>("MegaTPH")
                    .HasNoPropertyConditions()
                    .HasColumnCondition("MyDisc", "c");
            }
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_int16()
        {
            Requires_value_can_change_discriminator_column_name((Int16)1, (Int16)2, (Int16)3);
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_int32()
        {
            Requires_value_can_change_discriminator_column_name(1, 2, 3);
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_int64()
        {
            Requires_value_can_change_discriminator_column_name(1, 2, (Int64)3);
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_byte()
        {
            Requires_value_can_change_discriminator_column_name((byte)1, (byte)2, (byte)3);
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_sbyte()
        {
            sbyte a = 1, b = 2, c = 3;

            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(a).HasColumnType("tinyint"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue(b); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue(c); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", a);
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", b);
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", c);
        }

        [Fact]
        public void Requires_value_can_change_discriminator_column_name_and_use_bool()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(true); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue(false); });
            modelBuilder.Ignore<TPHLeaf>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "MyDisc");
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", true);
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", false);
        }

        [Fact]
        public void Requires_throws_on_decimal_discriminator()
        {
            Assert.Throws<MappingException>(() =>
                                            Requires_value_can_change_discriminator_column_name((decimal)1.0,
                                                                                                (decimal)2.0,
                                                                                                (decimal)3.0));
        }

        [Fact]
        public void Requires_throws_on_DateTime_discriminator()
        {
            Assert.Throws<MappingException>(() =>
                                            Requires_value_can_change_discriminator_column_name(
                                                new DateTime(2011, 1, 1), new DateTime(2011, 1, 2),
                                                new DateTime(2011, 1, 3)));
        }

        [Fact]
        public void Requires_throws_on_TimeSpan_discriminator()
        {
            Assert.Throws<MappingException>(() =>
                                            Requires_value_can_change_discriminator_column_name(new TimeSpan(1),
                                                                                                new TimeSpan(2),
                                                                                                new TimeSpan(3)));
        }

        [Fact]
        public void Requires_throws_on_a_nonPrimitive_discriminator()
        {
            Assert.Throws<ArgumentException>(() =>
                                             Requires_value_can_change_discriminator_column_name(
                                                 new FancyId { Id1 = 1, Id2 = 1 },
                                                 new FancyId { Id1 = 2, Id2 = 2 }, new FancyId { Id1 = 3, Id2 = 3 }));
        }

        [Fact]
        // Regression test for Dev11 Bug 136810
        public void Requires_can_only_be_called_once_per_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a"); });

            Assert.Throws<InvalidOperationException>(() =>
                                                     modelBuilder.Entity<TPHBase>()
                                                         .Map(mc => { mc.Requires("MyDisc").HasValue("b"); }));
        }

        [Fact]
        public void Requires_throws_on_mixed_valid_discriminator_types()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(1); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("yo"); });
            modelBuilder.Ignore<TPHLeaf>();

            Assert.Throws<MappingException>(() => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Requires_throws_on_ambiguous_valid_discriminator_types()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(1); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("2"); });
            modelBuilder.Ignore<TPHLeaf>();

            Assert.Throws<MappingException>(() => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Requires_throws_on_mixed_valid_discriminator_types_when_column_type_is_set()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(1).HasColumnType("int"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("yo"); });
            modelBuilder.Ignore<TPHLeaf>();

            Assert.Throws<MappingException>(() =>
                                            modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Requires_throws_on_conflicting_valid_discriminator_types_when_column_type_is_set()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(1).HasColumnType("int"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("yo").HasColumnType("ntext"); });
            modelBuilder.Ignore<TPHLeaf>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        private void Requires_value_can_change_discriminator_column_name<T>(T a, T b, T c) where T : struct
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue(a); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue(b); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue(c); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", a);
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", b);
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", c);
        }

        [Fact]
        public void Requires_value_can_be_null_with_nullable_string()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>()
                .Map(mc => { mc.Requires("MyDisc").HasValue("a"); })
                .Map<TPHDerived>(mc => { mc.Requires("MyDisc").HasValue("b"); })
                .Map<TPHLeaf>(mc => { mc.Requires("MyDisc").HasValue(null); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData", "LeafData", "MyDisc");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("MyDisc")
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHLeaf>("TPHBases")
                .HasNoPropertyConditions()
                .HasNullabilityColumnCondition("MyDisc", true);
        }

        [Fact]
        public void NotNull_condition_does_not_require_IsRequired_property()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>().Map(mc => mc.Requires("DerivedData").HasValue(null));
            modelBuilder.Entity<TPHDerived>().Map(mc => mc.Requires(b => b.DerivedData).HasValue());
            modelBuilder.Ignore<TPHLeaf>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.False(
                databaseMapping.Model.Namespaces[0].EntityTypes.Single(et => et.Name == "TPHDerived").Properties.Single(
                    p => p.Name == "DerivedData").PropertyType.IsNullable.Value);

            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("DerivedData")
                .DbFacetEqual(null, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHDerived>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasNullabilityColumnCondition("DerivedData", false);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasNullabilityColumnCondition("DerivedData", true);
        }

        [Fact]
        public void NotNull_condition_works_with_IsRequired()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>().Map(mc => mc.Requires("DerivedData").HasValue(null));
            modelBuilder.Entity<TPHDerived>().Map(mc => mc.Requires(b => b.DerivedData).HasValue());
            modelBuilder.Entity<TPHDerived>().Property(b => b.DerivedData).IsRequired();
            modelBuilder.Ignore<TPHLeaf>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.False(
                databaseMapping.Model.Namespaces[0].EntityTypes.Single(et => et.Name == "TPHDerived").Properties.Single(
                    p => p.Name == "DerivedData").PropertyType.IsNullable.Value);
            databaseMapping.Assert<TPHBase>()
                .HasColumns("Id", "BaseData", "IntProp", "NullableIntProp", "DerivedData");
            databaseMapping.Assert<TPHBase>("TPHBases")
                .Column("DerivedData")
                .DbFacetEqual(null, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHDerived>("TPHBases")
                .HasNoPropertyConditions()
                .HasNullabilityColumnCondition("DerivedData", false);
            databaseMapping.AssertMapping<TPHBase>("TPHBases", false)
                .HasNoPropertyConditions()
                .HasNullabilityColumnCondition("DerivedData", true);
        }

        [Fact]
        public void NotNull_condition_on_derived_with_abstract_base_in_TPH_and_base_condition_ignored()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBase>().Map(mc => mc.Requires("L1Data").HasValue(null));
            modelBuilder.Entity<AbsAtBaseL1>().Map(mc => mc.Requires(e => e.L1Data).HasValue());
            modelBuilder.Entity<AbsAtBaseL1>().Property(b => b.L1Data);
            modelBuilder.Ignore<AbsAtBaseL2>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.AssertMapping<AbsAtBase>("AbsAtBases").HasNoColumnConditions();
            databaseMapping.AssertMapping<AbsAtBaseL1>("AbsAtBases").HasNullabilityColumnCondition("L1Data", false);
        }

        [Fact]
        public void NotNull_condition_with_IsOptional_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>().Map(mc => mc.Requires("DerivedData").HasValue(null));
            modelBuilder.Entity<TPHDerived>().Map(mc => mc.Requires(b => b.DerivedData).HasValue());
            modelBuilder.Entity<TPHDerived>().Property(b => b.DerivedData).IsOptional();
            modelBuilder.Ignore<TPHLeaf>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Tree_hierarchy_can_map_to_same_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHRoot>()
                .Map(mc =>
                     {
                         mc.Requires("MyDisc").HasValue("a");
                         mc.ToTable("Woof");
                     })
                .Map<TPHNodeA>(mc =>
                               {
                                   mc.Requires("MyDisc").HasValue("b");
                                   mc.ToTable("Woof");
                               })
                .Map<TPHNodeB>(mc =>
                               {
                                   mc.Requires("MyDisc").HasValue("c");
                                   mc.ToTable("Woof");
                               });
            modelBuilder.Ignore<TPHNodeC>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHRoot>("Woof")
                .HasColumns("Id", "RootData", "AData", "BData", "MyDisc");
            databaseMapping.Assert<TPHRoot>("Woof")
                .Column("MyDisc")
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHRoot>("Woof", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "a");
            databaseMapping.AssertMapping<TPHNodeA>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "b");
            databaseMapping.AssertMapping<TPHNodeB>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("MyDisc", "c");
        }

        [Fact]
        public void Changing_only_root_table_moves_all_TPH_hierarchy()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHRoot>()
                .Map(mc => { mc.ToTable("Woof"); });
            modelBuilder.Ignore<TPHNodeC>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHRoot>("Woof")
                .HasColumns("Id", "RootData", "AData", "BData", "Discriminator");
            databaseMapping.Assert<TPHRoot>("Woof")
                .Column("Discriminator")
                .DbFacetEqual(DatabaseMappingGenerator.DiscriminatorLength, f => f.MaxLength);
            databaseMapping.AssertMapping<TPHRoot>("Woof", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHRoot");
            databaseMapping.AssertMapping<TPHNodeA>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHNodeA");
            databaseMapping.AssertMapping<TPHNodeB>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHNodeB");
        }

        [Fact]
        public void Null_discriminator_value_gets_string_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<IsolatedIsland>()
                .Map(mc => { mc.Requires("disc").HasValue(null); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<IsolatedIsland>()
                .HasColumns("Id", "Name", "disc")
                .DbEqual("nvarchar", tm => tm.Columns.Single(c => c.Name == "disc").TypeName)
                .DbEqual(128, tm => tm.Columns.Single(c => c.Name == "disc").Facets.MaxLength);
            databaseMapping.AssertMapping<IsolatedIsland>("IsolatedIslands")
                .HasNullabilityColumnCondition("disc", true);
        }

        [Fact]
        public void Null_discriminator_value_with_specified_length_gets_string_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<IsolatedIsland>()
                .Map(mc => { mc.Requires("disc").HasValue(null).HasMaxLength(100); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<IsolatedIsland>()
                .HasColumns("Id", "Name", "disc")
                .DbEqual("nvarchar", tm => tm.Columns.Single(c => c.Name == "disc").TypeName)
                .DbEqual(100, tm => tm.Columns.Single(c => c.Name == "disc").Facets.MaxLength);
            databaseMapping.AssertMapping<IsolatedIsland>("IsolatedIslands")
                .HasNullabilityColumnCondition("disc", true);
        }

        [Fact]
        public void FKs_in_base_type_remain_non_nullable_with_nullability_condition()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro136322_Principal>().HasKey(e => new
                                                                     {
                                                                         e.Key1,
                                                                         e.Key2,
                                                                     });
            modelBuilder.Entity<Repro136322_DerivedDependent>().Map(
                mapping => { mapping.Requires(e => e.Discriminator1).HasValue(); });
            modelBuilder.Entity<Repro136322_Dependent>().Map(
                mapping => { mapping.Requires("Discriminator1").HasValue(null); });
            modelBuilder.Entity<Repro136322_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany().
                HasForeignKey(dependent => new
                                           {
                                               dependent.PrincipalKey1,
                                               dependent.PrincipalKey2,
                                           });
            modelBuilder.Entity<Repro136322_Dependent>().Property(p => p.PrincipalKey1).IsRequired();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro136322_Dependent>()
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "PrincipalKey1").IsNullable)
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "PrincipalKey2").IsNullable);
        }

        [Fact]
        public void FKs_in_base_type_remain_non_nullable_with_nullability_condition_when_base_defined_first()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro136322_Principal>().HasKey(e => new
                                                                     {
                                                                         e.Key1,
                                                                         e.Key2,
                                                                     });
            modelBuilder.Entity<Repro136322_Dependent>().Map(
                mapping => { mapping.Requires("Discriminator1").HasValue(null); });
            modelBuilder.Entity<Repro136322_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany().
                HasForeignKey(dependent => new
                                           {
                                               dependent.PrincipalKey1,
                                               dependent.PrincipalKey2,
                                           });
            modelBuilder.Entity<Repro136322_Dependent>().Property(p => p.PrincipalKey1).IsRequired();
            modelBuilder.Entity<Repro136322_DerivedDependent>().Map(
                mapping => { mapping.Requires(e => e.Discriminator1).HasValue(); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro136322_Dependent>()
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "PrincipalKey1").IsNullable)
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "PrincipalKey2").IsNullable);
        }

        [Fact]
        // Repro for pending 136283
        public void Three_level_TPH_with_abstract_middle_has_nullable_discriminatorx()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<ThreeLevelBase>()
                .HasKey(e => e.Key1);
            modelBuilder.Entity<ThreeLevelBase>().Map(mapping =>
                                                      {
                                                          mapping.Requires("Discriminator2")
                                                              .HasValue("zbpmhdbllp")
                                                              .IsRequired()
                                                              .HasColumnType("nchar")
                                                              .HasMaxLength(30)
                                                              .IsUnicode()
                                                              .IsFixedLength();
                                                      });
            modelBuilder.Entity<ThreeLevelDerived>().Map(mapping =>
                                                         {
                                                             mapping.Requires("Discriminator2").HasValue("");
                                                             mapping.Requires("Discriminator1").HasValue(
                                                                 "analysismapping").HasColumnType("varchar").
                                                                 HasMaxLength(20).IsVariableLength();
                                                         });
            modelBuilder.Entity<ThreeLevelDerived>()
                .HasMany(e => e.DependentNavigation)
                .WithOptional()
                .Map(m => m.MapKey(new[] { "IndependentColumn1" }));

            modelBuilder.Entity<ThreeLevelLeaf>().Map(mapping =>
                                                      {
                                                          mapping.Requires("Discriminator2")
                                                              .HasValue("");
                                                          mapping.Requires("Discriminator1")
                                                              .HasValue("authenticode");
                                                      });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<ThreeLevelLeaf>()
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "Discriminator2").IsNullable)
                .DbEqual(true, tm => tm.Columns.Single(c => c.Name == "Discriminator1").IsNullable);
        }

        [Fact]
        public void Three_level_TPH_with_abstract_middle_with_IA_self_ref()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<ThreeLevelBase>().Map(
                mapping =>
                {
                    mapping.Requires("Discriminator2").HasValue("zbpmhdbllp").IsRequired().HasColumnType("nchar").
                        HasMaxLength(30).IsUnicode().IsFixedLength();
                });
            modelBuilder.Entity<ThreeLevelDerived>().Map(mapping =>
                                                         {
                                                             mapping.Requires("Discriminator2").HasValue("");
                                                             mapping.Requires("Discriminator1").HasValue(
                                                                 "analysismapping").HasColumnType("varchar").
                                                                 HasMaxLength(20).IsVariableLength();
                                                         });
            modelBuilder.Entity<ThreeLevelLeaf>().Map(mapping =>
                                                      {
                                                          mapping.Requires("Discriminator2").HasValue("");
                                                          mapping.Requires("Discriminator1").HasValue("authenticode");
                                                      });
            modelBuilder.Entity<ThreeLevelBase>().HasKey(e => e.Key1);
            modelBuilder.Entity<ThreeLevelDerived>()
                .HasMany(e => e.DependentNavigation)
                .WithOptional()
                .Map(m => m.MapKey(new[] { "IndependentColumn1" }));

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<ThreeLevelBase>("ThreeLevelBases")
                .HasColumns("Key1", "BaseProperty", "DerivedProperty1", "IndependentColumn1", "Discriminator2",
                            "Discriminator1");

            databaseMapping.AssertMapping<ThreeLevelBase>("ThreeLevelBases", false)
                .HasColumnCondition("Discriminator2", "zbpmhdbllp");

            databaseMapping.AssertNoMapping<ThreeLevelDerived>();

            databaseMapping.AssertMapping<ThreeLevelLeaf>("ThreeLevelBases")
                .HasColumnCondition("Discriminator2", "")
                .HasColumnCondition("Discriminator1", "authenticode");
        }

        [Fact]
        public void Derived_association_creates_nullable_FKs_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro135563_Principal>();
            modelBuilder.Entity<Repro135563_BaseDependent>();
            modelBuilder.Entity<Repro135563_Dependent>();

            modelBuilder.Entity<Repro135563_Principal>().HasOptional(e => e.DependentNavigation).WithRequired(
                e => e.PrincipalNavigation);
            modelBuilder.Entity<Repro135563_BaseDependent>().Property(p => p.BaseProperty).HasColumnType("time");
            modelBuilder.Entity<Repro135563_Principal>().HasKey(e => new
                                                                     {
                                                                         e.Key1,
                                                                         e.Key2,
                                                                     });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro135563_BaseDependent>()
                .DbEqual(true, tm => tm.Columns.Single(c => c.Name == "PrincipalNavigation_Key1").IsNullable)
                .DbEqual(true, tm => tm.Columns.Single(c => c.Name == "PrincipalNavigation_Key1").IsNullable);
        }

        [Fact]
        public void Base_FK_association_to_abstract_dependent_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsDep_Principal>().HasKey(e => e.Key1);
            modelBuilder.Entity<AbsDep_Dependent>()
                .HasRequired(e => e.PrincipalNavigation)
                .WithMany(e => e.DependentNavigation)
                .HasForeignKey(dependent => new { dependent.PrincipalKey1 });
            modelBuilder.Entity<AbsDep_DerivedDependent>().Map(
                mapping => { mapping.Requires("Discriminator1").HasValue("kssfjohr"); });
            modelBuilder.Entity<AbsDep_Dependent>().Map(
                mapping =>
                {
                    mapping.Requires("Discriminator1").HasValue("qrpnfdzhixxjujsgfsryoffpelbaxhtankvxlz").IsRequired().
                        HasColumnType("nvarchar").HasMaxLength(40).IsUnicode().IsVariableLength();
                });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AbsDep_Principal>("AbsDep_Principal")
                .HasColumns("Key1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<AbsDep_Dependent>("AbsDep_Dependent")
                .HasColumns("Id", "PrincipalKey1", "DerivedProperty1", "Discriminator1")
                .HasForeignKeyColumn("PrincipalKey1", "AbsDep_Principal");
        }

        [Fact]
        public void Base_IA_association_to_abstract_dependent_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsDep_Principal>().HasKey(e => e.Key1);
            modelBuilder.Entity<AbsDep_Dependent>()
                .HasRequired(e => e.PrincipalNavigation)
                .WithMany(e => e.DependentNavigation)
                .Map(im => im.MapKey("IndependentKey1"));
            modelBuilder.Entity<AbsDep_DerivedDependent>().Map(
                mapping => { mapping.Requires("Discriminator1").HasValue("kssfjohr"); });
            modelBuilder.Entity<AbsDep_Dependent>().Map(
                mapping =>
                {
                    mapping.Requires("Discriminator1").HasValue("qrpnfdzhixxjujsgfsryoffpelbaxhtankvxlz").IsRequired().
                        HasColumnType("nvarchar").HasMaxLength(40).IsUnicode().IsVariableLength();
                });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AbsDep_Principal>("AbsDep_Principal")
                .HasColumns("Key1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<AbsDep_Dependent>("AbsDep_Dependent")
                .HasColumns("Id", "PrincipalKey1", "DerivedProperty1", "IndependentKey1", "Discriminator1")
                .HasForeignKeyColumn("IndependentKey1", "AbsDep_Principal");
        }

        [Fact]
        public void Requires_property_is_sufficient_for_base()
        {
            var modelBuilder = new AdventureWorksModelBuilder();
            modelBuilder.Entity<Repro143236_Dependent>();
            modelBuilder.Entity<Repro143236_DerivedDependent>().Map(
                mapping => { mapping.Requires(e => e.DiscriminatorNotNull).HasValue(); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void Condition_column_configuration_on_abstract_base_applied_to_column()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro142666_Dependent>().Map(
                mapping =>
                {
                    mapping.Requires("DiscriminatorValue").HasValue("2").IsRequired().HasColumnType("nchar").
                        HasMaxLength(30).IsUnicode().IsFixedLength();
                });
            modelBuilder.Entity<Repro142666_DerivedDependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue("1"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro142666_DerivedDependent>()
                .DbEqual("nchar", t => t.Columns.Single(c => c.Name == "DiscriminatorValue").TypeName)
                .DbEqual(30, t => t.Columns.Single(c => c.Name == "DiscriminatorValue").Facets.MaxLength);
        }

        [Fact]
        public void TPH_with_an_IA_from_abstract_non_root_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro142682_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany(
                e => e.DependentNavigation);
            modelBuilder.Entity<Repro142682_Dependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue("2"); });
            modelBuilder.Entity<Repro142682_DerivedDependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue("1"); });
            modelBuilder.Entity<Repro142682_BaseDependent>().Map(
                mapping =>
                {
                    mapping.Requires("DiscriminatorValue").HasValue("3").IsRequired().HasColumnType("nvarchar(max)").
                        IsUnicode();
                });
            modelBuilder.Entity<Repro142682_Principal>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void TPH_with_an_FK_from_abstract_root_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro150248_BasePrincipal_2>().HasKey(e => e.Key1);
            modelBuilder.Entity<Repro150248_Dependent_2>().HasKey(e => e.Key1);
            modelBuilder.Entity<Repro150248_Principal_2>().HasOptional(e => e.DependentNavigation).WithRequired(
                e => e.PrincipalNavigation);
            modelBuilder.Entity<Repro150248_BasePrincipal_2>().Map(mapping => { mapping.ToTable("BasePrincipal"); });
            modelBuilder.Entity<Repro150248_Principal_2>().Map(mapping => { mapping.ToTable("Principal"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void TPH_with_self_ref_FK_on_derived_type_has_non_nullable_FK_when_base_type_is_abstract()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro135563_2_Dependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue(1).HasColumnType("tinyint"); });
            modelBuilder.Entity<Repro135563_2_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany(
                e => e.DependentNavigation).Map(m => { });
            modelBuilder.Entity<Repro135563_2_BaseDependent>().HasKey(e => e.Key1);
            modelBuilder.Ignore<Repro135563_2_Other>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro135563_2_Dependent>()
                .HasColumns("Key1", "BaseProperty", "PrincipalNavigation_Key1", "DiscriminatorValue")
                .HasForeignKeyColumn("PrincipalNavigation_Key1")
                .DbEqual(false, t => t.Columns.Single(c => c.Name == "PrincipalNavigation_Key1").IsNullable);
        }

        [Fact]
        public void TPH_with_self_ref_FK_on_derived_type_has_nullable_FK_when_base_type_is_abstract_but_there_are_other_nonrelated_derived_types()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro135563_2_Dependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue(1).HasColumnType("tinyint"); });
            modelBuilder.Entity<Repro135563_2_Other>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue(0); });
            modelBuilder.Entity<Repro135563_2_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany(
                e => e.DependentNavigation).Map(m => { });
            modelBuilder.Entity<Repro135563_2_BaseDependent>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro135563_2_Dependent>()
                .HasColumns("Key1", "BaseProperty", "OtherData", "PrincipalNavigation_Key1", "DiscriminatorValue")
                .HasForeignKeyColumn("PrincipalNavigation_Key1")
                .DbEqual(true, t => t.Columns.Single(c => c.Name == "PrincipalNavigation_Key1").IsNullable);
        }

        [Fact]
        public void TPH_with_self_ref_FK_on_derived_type_has_nullable_FK_when_base_type_is_concrete()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro135563_3_BaseDependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue(0).HasColumnType("tinyint"); });
            modelBuilder.Entity<Repro135563_3_Dependent>().Map(
                mapping => { mapping.Requires("DiscriminatorValue").HasValue(1); });
            modelBuilder.Entity<Repro135563_3_Dependent>().HasRequired(e => e.PrincipalNavigation).WithMany(
                e => e.DependentNavigation).Map(m => { });
            modelBuilder.Entity<Repro135563_3_BaseDependent>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro135563_3_Dependent>()
                .HasColumns("Key1", "BaseProperty", "PrincipalNavigation_Key1", "DiscriminatorValue")
                .HasForeignKeyColumn("PrincipalNavigation_Key1")
                .DbEqual(true, t => t.Columns.Single(c => c.Name == "PrincipalNavigation_Key1").IsNullable);
        }

        #endregion

        #region Basic TPT Tests

        [Fact]
        public void ToTable_configures_basic_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().ToTable("Deriveds");
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2,
                         databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.
                             Count);

            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Deriveds");
        }

        [Fact]
        public void ToTable_configures_TPT_with_unmapped_abstract_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBaseL1>().ToTable("L1");
            modelBuilder.Entity<AbsAtBaseL2>().ToTable("L2");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2,
                         databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.
                             Count);
            databaseMapping.Assert<AbsAtBaseL1>("L1");
            databaseMapping.Assert<AbsAtBaseL2>("L2");
        }

        [Fact]
        public void ToTable_configures_TPT_with_mapped_abstract_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBase>().ToTable("Bases");
            modelBuilder.Entity<AbsAtBaseL1>().ToTable("L1");
            modelBuilder.Entity<AbsAtBaseL2>().ToTable("L2");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(3,
                         databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.
                             Count);

            databaseMapping.Assert<AbsAtBase>("Bases");
            databaseMapping.Assert<AbsAtBaseL1>("L1");
            databaseMapping.Assert<AbsAtBaseL2>("L2");
        }

        [Fact]
        public void ToTable_configures_TPT_with_FK_on_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);
            modelBuilder.Entity<AssocDerived>().ToTable("Derived");
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Deriveds);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Derived");
            databaseMapping.Assert<AssocBase>().HasForeignKeyColumn("AssocRelatedBaseId");
        }

        [Fact]
        public void ToTable_configures_TPT_with_FK_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().ToTable("Derived");
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Bases);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Derived");
            databaseMapping.Assert<AssocDerived>().HasForeignKeyColumn("AssocRelatedId");
        }

        [Fact]
        public void ToTable_configures_TPT_with_IA_on_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);
            modelBuilder.Entity<AssocDerived>().ToTable("Derived");
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Deriveds);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Derived");
            databaseMapping.Assert<AssocBase>().HasForeignKeyColumn("AssocRelatedBase_Id");
        }

        [Fact]
        public void ToTable_configures_TPT_with_IA_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().ToTable("Derived");
            modelBuilder.Entity<AssocDerived>().Ignore(x => x.AssocRelatedId);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Bases);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Derived");
            databaseMapping.Assert<AssocDerived>().HasForeignKeyColumn("AssocRelated_Id");
        }

        [Fact]
        public void ToTable_configures_TPT_with_IA_on_derived_type_with_configured_relationship()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocDerived>().HasRequired(e => e.AssocRelated).WithOptional(e => e.RefDerived);

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);

            modelBuilder.Entity<AssocDerived>().ToTable("Derived");
            modelBuilder.Entity<AssocDerived>().Ignore(x => x.AssocRelatedId);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Bases);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Deriveds);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("Bases");
            databaseMapping.Assert<AssocDerived>("Derived");
            // 1:0..1 makes Id the FK for the AssocRelated to AssocDerived relationship
            databaseMapping.Assert<AssocDerived>().DbEqual(2, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<AssocDerived>().HasForeignKeyColumn("Id", "Bases");
            databaseMapping.Assert<AssocDerived>().HasForeignKeyColumn("Id", "AssocRelateds");
        }

        [Fact]
        public void ToTable_configures_TPT_and_sets_FK_to_base_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHBase>().ToTable("A");
            modelBuilder.Entity<TPHDerived>().ToTable("B");
            modelBuilder.Entity<TPHLeaf>().ToTable("C");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<TPHDerived>("B").HasForeignKeyColumn("Id", "A")
                .DbEqual(1, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TPHLeaf>("C").HasForeignKeyColumn("Id", "B")
                .DbEqual(1, t => t.ForeignKeyConstraints.Count);
        }

        [Fact]
        public void ToTable_configures_TPT_and_creates_single_FK_to_related()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<CKCategory>().Property(p => p.Key1).HasColumnType("uniqueidentifier");
            modelBuilder.Entity<CKCategory>().Map(mapping => { mapping.ToTable("Principal"); });
            modelBuilder.Entity<CKCategory>().HasKey(e => new
                                                          {
                                                              e.Key1,
                                                              e.Key2,
                                                          });

            modelBuilder.Entity<CKImage>().Property(p => p.Key1).HasColumnType("uniqueidentifier");
            modelBuilder.Entity<CKImage>().Map(mapping => { mapping.ToTable("Images"); });
            modelBuilder.Entity<CKImage>().HasKey(e => new
                                                       {
                                                           e.Key1,
                                                           e.Key2,
                                                       });

            modelBuilder.Entity<CKProduct>().HasRequired(e => e.CKCategory).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<CKProduct>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<CKProduct>().HasKey(e => new
                                                         {
                                                             e.CKCategoryKey1,
                                                             e.CKCategoryKey2,
                                                         });

            modelBuilder.Entity<CKDerivedProduct>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<CKDerivedProduct>().HasRequired(dp => dp.Image).WithOptional();

            modelBuilder.Entity<CKDerivedProduct2>().Map(mapping => { mapping.ToTable("DerivedDependent2"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<CKCategory>()
                .DbEqual(0, t => t.ForeignKeyConstraints.Count);

            databaseMapping.Assert<CKProduct>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Principal");

            databaseMapping.Assert<CKDerivedProduct>()
                .DbEqual(2, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Dependent")
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Images");

            databaseMapping.Assert<CKDerivedProduct2>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "DerivedDependent");
        }

        [Fact]
        public void ToTable_configures_TPT_and_creates_single_FK_to_self_reference()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SelfRefBase>().HasKey(e => e.Key1);
            modelBuilder.Entity<SelfRefBase>().Map(mapping => { mapping.ToTable("SelfRefBase"); });
            modelBuilder.Entity<SelfRefDerived>().Map(mapping => { mapping.ToTable("SelfRefDerived"); });
            modelBuilder.Entity<SelfRefDerived>().HasOptional(e => e.SelfRefNavigation).WithMany().HasForeignKey(
                d => new
                     {
                         d.DependentKey1
                     }).WillCascadeOnDelete(false);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<SelfRefBase>()
                .DbEqual(0, t => t.ForeignKeyConstraints.Count);

            databaseMapping.Assert<SelfRefDerived>()
                .DbEqual(2, t => t.ForeignKeyConstraints.Count)
                .HasForeignKeyColumn("Key1", "SelfRefBase")
                .HasForeignKeyColumn("DependentKey1", "SelfRefDerived");
        }

        [Fact]
        public void TPT_moves_FKs_pointing_to_moved_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro136761_A>().ToTable("A");
            modelBuilder.Entity<Repro136761_B>().ToTable("B");
            modelBuilder.Entity<Repro136761_C>().ToTable("C");
            modelBuilder.Entity<Repro136761_C>().HasRequired(c => c.Repro136761_B).WithMany().HasForeignKey(
                c => c.Repro136761_BId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro136761_A>()
                .DbEqual(0, t => t.ForeignKeyConstraints.Count);

            databaseMapping.Assert<Repro136761_B>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKeyColumn("Repro136761_AId", "A");

            databaseMapping.Assert<Repro136761_C>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKeyColumn("Repro136761_BId", "B");
        }

        [Fact]
        public void FKs_in_base_type_remain_non_nullable_with_nullability_condition_in_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro136855_DerivedDependent>().Property(p => p.DerivedProperty1).HasColumnType("bit");
            modelBuilder.Entity<Repro136855_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<Repro136855_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<Repro136855_Principal>().Property(p => p.Key1).HasColumnType("smalldatetime");
            modelBuilder.Entity<Repro136855_Principal>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro136855_Dependent>()
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "Key1").IsNullable);
        }

        [Fact]
        public void FKs_in_base_type_remain_non_nullable_with_nullability_condition_when_base_defined_first_in_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro136855_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<Repro136855_DerivedDependent>().Property(p => p.DerivedProperty1).HasColumnType("bit");
            modelBuilder.Entity<Repro136855_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<Repro136855_Principal>().Property(p => p.Key1).HasColumnType("smalldatetime");
            modelBuilder.Entity<Repro136855_Principal>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro136855_Dependent>()
                .DbEqual(false, tm => tm.Columns.Single(c => c.Name == "Key1").IsNullable);
        }

        [Fact]
        public void IA_Association_between_subtypes_with_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<THABS_BasePrincipal>().HasKey(e => e.Key1);
            modelBuilder.Entity<THABS_Principal>().HasMany(e => e.THABS_DependentNavigation);
            modelBuilder.Entity<THABS_Dependent>().HasOptional(e => e.THABS_PrincipalNavigation);
            modelBuilder.Entity<THABS_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<THABS_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<THABS_BasePrincipal>().Map(mapping => { mapping.ToTable("BasePrincipal"); });
            modelBuilder.Entity<THABS_Principal>().Map(mapping => { mapping.ToTable("Principal"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<THABS_BasePrincipal>("BasePrincipal")
                .HasColumns("Key1", "BaseProperty")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<THABS_Principal>("Principal")
                .HasColumns("Key1")
                .HasForeignKeyColumn("Key1", "BasePrincipal");

            databaseMapping.Assert<THABS_Dependent>("Dependent")
                .HasColumns("Id", "THABS_PrincipalNavigation_Key1")
                .HasForeignKeyColumn("THABS_PrincipalNavigation_Key1", "Principal");

            databaseMapping.Assert<THABS_DerivedDependent>("DerivedDependent")
                .HasColumns("Id", "DerivedProperty1")
                .HasForeignKeyColumn("Id", "Dependent");
        }

        [Fact]
        public void Self_referencing_relationship_on_derived_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SRBase>().Map(mapping => { mapping.ToTable("Base"); });
            modelBuilder.Entity<SRBase>().HasKey(e => new
                                                      {
                                                          e.Key1,
                                                          e.Key2,
                                                      });
            modelBuilder.Entity<SRDerived>().HasMany(e => e.Navigation).WithMany();
            modelBuilder.Entity<SRDerived>().Map(mapping => { mapping.ToTable("Derived"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<SRBase>("Base")
                .HasColumns("Key1", "Key2", "BaseProperty")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<SRDerived>("Derived")
                .HasColumns("Key1", "Key2")
                .HasForeignKey(new[] { "Key1", "Key2" }, "Base");

            var joinTable = databaseMapping.Database.Schemas[0].Tables[1];
            Assert.Equal("SRDerivedSRDeriveds", joinTable.DatabaseIdentifier);
            Assert.True(
                joinTable.Columns.Select(x => x.Name).SequenceEqual(new[]
                                                                    {
                                                                        "SRDerived_Key1", "SRDerived_Key2",
                                                                        "SRDerived_Key11", "SRDerived_Key21"
                                                                    }));
            Assert.Equal(2, joinTable.ForeignKeyConstraints.Count);
            Assert.True(joinTable.ForeignKeyConstraints.All(fk => fk.PrincipalTable.DatabaseIdentifier == "Derived"));
        }

        [Fact]
        public void ToTable_TPT_to_same_base_table_results_in_default_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro137329_A1>().ToTable("A1");
            modelBuilder.Entity<Repro137329_A2>().ToTable("A1");
            modelBuilder.Entity<Repro137329_A3>().ToTable("A1");
            modelBuilder.Entity<Repro137329_A4>().ToTable("A1");
            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.AssertMapping<Repro137329_A1>("A1", false)
                .HasColumnCondition("Discriminator", "Repro137329_A1");
        }

        [Fact]
        public void IA_between_TPT_subtypes_has_FK_to_sub_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro109944_BaseEntity>().ToTable("BaseEntities");
            modelBuilder.Entity<Repro109944_Entity1>().ToTable("Entity1s");
            modelBuilder.Entity<Repro109944_Entity1>().Ignore(x => x.Repro109944_Entity2ID);
            modelBuilder.Entity<Repro109944_Entity2>().ToTable("Entity2s");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro109944_BaseEntity>()
                .HasNoForeignKeyColumns();
            databaseMapping.Assert<Repro109944_Entity1>()
                .HasForeignKeyColumn("ID", "BaseEntities")
                .HasForeignKeyColumn("Repro109944_Entity2_ID", "Entity2s");
            databaseMapping.Assert<Repro109944_Entity2>()
                .HasForeignKeyColumn("ID", "BaseEntities");
        }

        [Fact]
        public void FK_between_TPT_subtypes_has_FK_to_sub_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro109944_BaseEntity>().ToTable("BaseEntities");
            modelBuilder.Entity<Repro109944_Entity1>().ToTable("Entity1s");
            modelBuilder.Entity<Repro109944_Entity2>().ToTable("Entity2s");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro109944_BaseEntity>()
                .HasNoForeignKeyColumns();
            databaseMapping.Assert<Repro109944_Entity1>()
                .HasForeignKeyColumn("ID", "BaseEntities")
                .HasForeignKeyColumn("Repro109944_Entity2ID", "Entity2s");
            databaseMapping.Assert<Repro109944_Entity2>()
                .HasForeignKeyColumn("ID", "BaseEntities");
        }

        [Fact]
        public void FK_between_TPT_base_types_has_FK_to_base_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro142961_DerivedDependent>();
            modelBuilder.Entity<Repro142961_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<Repro142961_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<Repro142961_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            modelBuilder.Entity<Repro142961_DerivedPrincipal>().Map(mapping => { mapping.ToTable("DerivedPrincipal"); });
            modelBuilder.Entity<Repro142961_Principal>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro142961_Dependent>("Dependent")
                .HasForeignKeyColumn("Key1", "Principal");
        }

        [Fact]
        public void IA_between_TPT_base_types_has_FK_to_base_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro142961_DerivedDependent>();
            modelBuilder.Entity<Repro142961_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<Repro142961_Dependent>().Ignore(x => x.Key1);
            modelBuilder.Entity<Repro142961_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            modelBuilder.Entity<Repro142961_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            modelBuilder.Entity<Repro142961_DerivedPrincipal>().Map(mapping => { mapping.ToTable("DerivedPrincipal"); });
            modelBuilder.Entity<Repro142961_Principal>().HasKey(e => e.Key1);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro142961_Dependent>("Dependent")
                .HasForeignKeyColumn("PrincipalNavigation_Key1", "Principal");
        }

        [Fact]
        public void TPT_with_multiple_associations_in_hierarchy()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro109916_BaseEntity>().ToTable("BaseEntities");
            modelBuilder.Entity<Repro109916_SomeEntity>().ToTable("SomeEntities");
            modelBuilder.Entity<Repro109916_SomeMore>().ToTable("SomeMores");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void TPT_with_asociation_between_middle_types()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro150248_Principal>().HasMany(e => e.DependentNavigation);
            // Works if this line is commented out.
            builder.Entity<Repro150248_DerivedPrincipal>().Property(p => p.DerivedProperty1).HasColumnType(
                "nvarchar(max)").IsUnicode();
            builder.Entity<Repro150248_Dependent>().HasRequired(e => e.PrincipalNavigation);
            builder.Entity<Repro150248_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            builder.Entity<Repro150248_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            builder.Entity<Repro150248_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            builder.Entity<Repro150248_BaseDependent>().Property(p => p.BaseProperty).HasColumnType("date");
            builder.Entity<Repro150248_BaseDependent>().Map(mapping => { mapping.ToTable("BaseDependent"); });
            builder.Entity<Repro150248_DerivedDependent>().Property(p => p.DerivedProperty1).HasColumnType("smallmoney");
            builder.Entity<Repro150248_DerivedPrincipal>().Map(mapping => { mapping.ToTable("DerivedPrincipal"); });
            builder.Entity<Repro150248_BaseDependent>().HasKey(e => e.Key1);
            builder.Entity<Repro150248_Dependent>().Property(p => p.PrincipalKey1).IsRequired();

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void TPT_with_no_asociation_between_middle_types()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro150248_Principal>().Ignore(e => e.DependentNavigation);
            // Works if this line is commented out.
            builder.Entity<Repro150248_DerivedPrincipal>().Property(p => p.DerivedProperty1).HasColumnType(
                "nvarchar(max)").IsUnicode();
            builder.Entity<Repro150248_Dependent>().Ignore(e => e.PrincipalNavigation);
            builder.Entity<Repro150248_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            builder.Entity<Repro150248_DerivedDependent>().Map(mapping => { mapping.ToTable("DerivedDependent"); });
            builder.Entity<Repro150248_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            builder.Entity<Repro150248_BaseDependent>().Property(p => p.BaseProperty).HasColumnType("date");
            builder.Entity<Repro150248_BaseDependent>().Map(mapping => { mapping.ToTable("BaseDependent"); });
            builder.Entity<Repro150248_DerivedDependent>().Property(p => p.DerivedProperty1).HasColumnType("smallmoney");
            builder.Entity<Repro150248_DerivedPrincipal>().Map(mapping => { mapping.ToTable("DerivedPrincipal"); });
            builder.Entity<Repro150248_BaseDependent>().HasKey(e => e.Key1);
            builder.Entity<Repro150248_Dependent>().Property(p => p.PrincipalKey1).IsRequired();

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void TPT_with_required_to_optional_IA_on_derived_type_creates_non_nullable_FK()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro150634_BaseDependent>().HasKey(e => new
                                                                    {
                                                                        e.Key1,
                                                                        e.Key2,
                                                                    });
            builder.Entity<Repro150634_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            builder.Entity<Repro150634_BaseDependent>().Map(mapping => { mapping.ToTable("BaseDependent"); });
            builder.Entity<Repro150634_Dependent>().HasRequired(e => e.PrincipalNavigation).WithOptional(
                e => e.DependentNavigation)
                .Map(_ => { }).WillCascadeOnDelete(false);

            builder.Entity<Repro150634_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            builder.Ignore<Repro150634_DerivedDependent>();

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro150634_Dependent>("Dependent")
                .DbEqual(false, t => t.Columns.Single(x => x.Name == "PrincipalNavigation_Id").IsNullable);
        }

        [Fact]
        public void TPT_with_required_to_optional_IA_on_derived_middle_type_creates_non_nullable_FK()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro150634_BaseDependent>().HasKey(e => new
                                                                    {
                                                                        e.Key1,
                                                                        e.Key2,
                                                                    });
            builder.Entity<Repro150634_Dependent>().Map(mapping => { mapping.ToTable("Dependent"); });
            builder.Entity<Repro150634_BaseDependent>().Map(mapping => { mapping.ToTable("BaseDependent"); });
            builder.Entity<Repro150634_Dependent>().HasRequired(e => e.PrincipalNavigation).WithOptional(
                e => e.DependentNavigation)
                .Map(_ => { }).WillCascadeOnDelete(false);

            builder.Entity<Repro150634_Principal>().Map(mapping => { mapping.ToTable("Principal"); });
            builder.Entity<Repro150634_DerivedDependent>().ToTable("DerivedDependent");

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<Repro150634_Dependent>("Dependent")
                .DbEqual(false, t => t.Columns.Single(x => x.Name == "PrincipalNavigation_Id").IsNullable);
        }

        [Fact]
        public void Uniquification_happens_correctly_when_relationship_with_same_name_exists_in_two_derived_types()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<BaseEmployee>().ToTable("BaseEmployee");
            modelBuilder.Entity<TargetEmployee>().ToTable("TargetEmployee");
            modelBuilder.Entity<BaseNote>().ToTable("Note");
            modelBuilder.Entity<NoteWithRelationship1>().ToTable("NoteWithRelationship1");
            modelBuilder.Entity<NoteWithRelationship1>().HasRequired(x => x.TargetEmployee);
            modelBuilder.Entity<NoteWithRelationship2>().ToTable("NoteWithRelationship2");
            modelBuilder.Entity<NoteWithRelationship2>().HasRequired(x => x.TargetEmployee);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Uniquification_happens_correctly_when_complex_type_reused_in_derived_types()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<VehicleProduct>();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        #endregion

        #region Basic TPC Tests

        [Fact]
        public void Two_entity_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc => mc.ToTable("Bases"));
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2,
                         databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.
                             Count);
            databaseMapping.Assert<AssocBase>("Bases").HasColumns("Id", "Name", "BaseData");
            databaseMapping.AssertMapping<AssocBase>("Bases", false);
            databaseMapping.Assert<AssocDerived>("Deriveds").HasColumns("Id", "Name", "BaseData", "DerivedData1",
                                                                        "DerivedData2");
            databaseMapping.AssertMapping<AssocDerived>("Deriveds", false);
        }

        [Fact]
        public void Two_entity_TPC_with_multiple_nonconflicting_configurations()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc => mc.ToTable("Bases"));
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);

            Assert.Throws<InvalidOperationException>(() =>
                                                     modelBuilder.Entity<AssocDerived>()
                                                         .Map(mc => { mc.Properties(d => d.DerivedData1); })
                                                         .Map(mc => { mc.Properties(d => d.DerivedData2); })
                                                         .Map(mc => { mc.MapInheritedProperties(); })
                                                         .Map(mc => { mc.ToTable("Deriveds"); }));
        }

        [Fact]
        public void Two_entity_TPC_with_column_override()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc => mc.ToTable("Bases"))
                .Property(a => a.Name).HasColumnType("char");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2,
                         databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.
                             Count);
            databaseMapping.Assert<AssocBase>("Bases").HasColumns("Id", "Name", "BaseData");
            databaseMapping.Assert<AssocBase>("Bases").DbEqual("char",
                                                               x => x.Columns.Single(c => c.Name == "Name").TypeName);
            databaseMapping.Assert<AssocDerived>("Deriveds").HasColumns("Id", "Name", "BaseData", "DerivedData1",
                                                                        "DerivedData2");
            databaseMapping.Assert<AssocDerived>("Deriveds").DbEqual("char",
                                                                     x =>
                                                                     x.Columns.Single(c => c.Name == "Name").TypeName);
        }

        //[Fact]
        //public void ToTable_configures_TPT_with_unmapped_abstract_base_type()
        //{
        //    var modelBuilder = new AdventureWorksModelBuilder();

        //    modelBuilder.Entity<AbsAtBaseL1>().ToTable("L1");
        //    modelBuilder.Entity<AbsAtBaseL2>().ToTable("L2");

        //    var databaseMapping = modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo).DatabaseMapping;

        //    Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        //    Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.Count);
        //    Assert.True(databaseMapping.Database.Schemas.Single().Tables.Any(t => t.Name == "L1"));
        //    Assert.True(databaseMapping.Database.Schemas.Single().Tables.Any(t => t.Name == "L2"));
        //}

        //[Fact]
        //public void ToTable_configures_TPT_with_mapped_abstract_base_type()
        //{
        //    var modelBuilder = new AdventureWorksModelBuilder();

        //    modelBuilder.Entity<AbsAtBase>().ToTable("Bases");
        //    modelBuilder.Entity<AbsAtBaseL1>().ToTable("L1");
        //    modelBuilder.Entity<AbsAtBaseL2>().ToTable("L2");

        //    var databaseMapping = modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo).DatabaseMapping;

        //    Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        //    Assert.Equal(3, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Single().EntityTypeMappings.Count);
        //    Assert.True(databaseMapping.Database.Schemas.Single().Tables.Any(t => t.Name == "Bases"));
        //    Assert.True(databaseMapping.Database.Schemas.Single().Tables.Any(t => t.Name == "L1"));
        //    Assert.True(databaseMapping.Database.Schemas.Single().Tables.Any(t => t.Name == "L2"));
        //}

        [Fact]
        public void ToTable_configures_TPC_with_FK_on_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Deriveds);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AssocBase>("Bases").HasColumns("Id", "Name", "BaseData", "AssocRelatedBaseId");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasColumns("Id", "Name", "BaseData", "AssocRelatedBaseId",
                                                                        "DerivedData1", "DerivedData2");
            databaseMapping.Assert<AssocBase>("Bases").HasForeignKeyColumn("AssocRelatedBaseId");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasForeignKeyColumn("AssocRelatedBaseId");
        }

        [Fact]
        public void ToTable_configures_TPC_with_FK_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Bases);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AssocBase>("Bases").HasColumns("Id", "Name", "BaseData");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasColumns("Id", "Name", "BaseData", "AssocRelatedId",
                                                                        "DerivedData1", "DerivedData2");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasForeignKeyColumn("AssocRelatedId");
        }

        [Fact]
        public void ToTable_configures_TPC_with_IA_on_derived_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.Bases);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefBase);
            modelBuilder.Entity<AssocRelated>().Ignore(r => r.RefDerived);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<AssocBase>("Bases").HasColumns("Id", "Name", "BaseData");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasColumns("Id", "AssocRelated_Id", "Name", "BaseData",
                                                                        "DerivedData1", "DerivedData2");
            databaseMapping.Assert<AssocDerived>("Deriveds").HasForeignKeyColumn("AssocRelated_Id");
        }

        [Fact]
        public void ToTable_configures_TPC_and_creates_single_FK_to_related()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<CKCategory>().Property(p => p.Key1).HasColumnType("uniqueidentifier");
            modelBuilder.Entity<CKCategory>().Map(mapping => { mapping.ToTable("Principal"); });
            modelBuilder.Entity<CKCategory>().HasKey(e => new
                                                          {
                                                              e.Key1,
                                                              e.Key2,
                                                          });
            modelBuilder.Entity<CKProduct>().HasRequired(e => e.CKCategory).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<CKProduct>().Map(mapping => { mapping.ToTable("Dependent"); });
            modelBuilder.Entity<CKProduct>().HasKey(e => new
                                                         {
                                                             e.CKCategoryKey1,
                                                             e.CKCategoryKey2,
                                                         });

            modelBuilder.Entity<CKDerivedProduct>().Map(mapping =>
                                                        {
                                                            mapping.MapInheritedProperties();
                                                            mapping.ToTable("DerivedDependent");
                                                        });
            modelBuilder.Entity<CKDerivedProduct2>().Map(mapping =>
                                                         {
                                                             mapping.MapInheritedProperties();
                                                             mapping.ToTable("DerivedDependent2");
                                                         });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<CKCategory>()
                .DbEqual(0, t => t.ForeignKeyConstraints.Count);

            databaseMapping.Assert<CKProduct>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Principal");

            databaseMapping.Assert<CKDerivedProduct>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Principal");

            databaseMapping.Assert<CKDerivedProduct2>()
                .DbEqual(1, t => t.ForeignKeyConstraints.Count)
                .HasForeignKey(new[] { "CKCategoryKey1", "CKCategoryKey2" }, "Principal");
        }

        [Fact]
        public void Mapping_store_type_propagates_to_derived_TPC_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<ByteBase>();
            modelBuilder.Entity<ByteDerived>().Property(x => x.DerivedData).HasColumnType("image");
            modelBuilder.Entity<ByteDerived>()
                .Map(mc =>
                     {
                         mc.ToTable("Derived");
                         mc.MapInheritedProperties();
                     });
            modelBuilder.Entity<ByteDerived2>()
                .Map(mc =>
                     {
                         mc.ToTable("Derived2");
                         mc.MapInheritedProperties();
                     });
            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            databaseMapping.Assert<ByteDerived>().DbEqual("image",
                                                          t => t.Columns.Single(c => c.Name == "DerivedData").TypeName);
            databaseMapping.Assert<ByteDerived2>().DbEqual("image",
                                                           t => t.Columns.Single(c => c.Name == "DerivedData").TypeName);
        }

        [Fact]
        public void FK_association_to_principal_base_should_not_make_FK_in_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();
            modelBuilder.Entity<AssocBase_Employee>();

            modelBuilder.Entity<AssocBase_Department>()
                .Map(m => m.ToTable("Department"))
                .Map<AssocBase_OldDepartment>(m =>
                                              {
                                                  m.MapInheritedProperties();
                                                  m.ToTable("OldDepartment");
                                              });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<AssocBase_Employee>().HasNoForeignKeyColumns();
            databaseMapping.Assert<AssocBase_Department>().HasNoForeignKeyColumns();
            databaseMapping.Assert<AssocBase_OldDepartment>().HasNoForeignKeyColumns();
        }

        [Fact]
        public void Self_referencing_relationship_on_derived_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SRBase>().Map(mapping => { mapping.ToTable("Base"); });
            modelBuilder.Entity<SRBase>().HasKey(e => new
                                                      {
                                                          e.Key1,
                                                          e.Key2,
                                                      });
            modelBuilder.Entity<SRDerived>().HasMany(e => e.Navigation).WithMany();
            modelBuilder.Entity<SRDerived>().Map(mapping =>
                                                 {
                                                     mapping.MapInheritedProperties();
                                                     mapping.ToTable("Derived");
                                                 });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<SRBase>("Base")
                .HasColumns("Key1", "Key2", "BaseProperty")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<SRDerived>("Derived")
                .HasColumns("Key1", "Key2", "BaseProperty")
                .HasNoForeignKeyColumns();

            var joinTable = databaseMapping.Database.Schemas[0].Tables[1];
            Assert.Equal("SRDerivedSRDeriveds", joinTable.DatabaseIdentifier);
            Assert.True(
                joinTable.Columns.Select(x => x.Name).SequenceEqual(new[]
                                                                    {
                                                                        "SRDerived_Key1", "SRDerived_Key2",
                                                                        "SRDerived_Key11", "SRDerived_Key21"
                                                                    }));
        }

        [Fact]
        public void TPC_with_abstract_base_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro143662_Party>()
                .Map<Repro143662_Agency>(m =>
                                         {
                                             m.MapInheritedProperties();
                                             m.ToTable("Agencies");
                                         })
                .Map<Repro143662_Person>(m =>
                                         {
                                             m.MapInheritedProperties();
                                             m.ToTable("People");
                                         });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Column_type_configuration_on_non_root_type_is_propagated_to_derived_types()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro143127_EntityB>().Property(e => e.Info).HasColumnType("varchar(max)");
            modelBuilder.Entity<Repro143127_EntityA>().Map(
                m => { m.ToTable("Table1"); });
            modelBuilder.Entity<Repro143127_EntityB>().Map(
                m =>
                {
                    m.MapInheritedProperties();
                    m.ToTable("Table2");
                });
            modelBuilder.Entity<Repro143127_EntityC>().Map(
                m =>
                {
                    m.MapInheritedProperties();
                    m.ToTable("Table3");
                });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro143127_EntityB>("Table2")
                .DbEqual("varchar(max)", t => t.Columns.Single(c => c.Name == "Info").TypeName);
            databaseMapping.Assert<Repro143127_EntityC>("Table3")
                .DbEqual("varchar(max)", t => t.Columns.Single(c => c.Name == "Info").TypeName);
        }

        [Fact]
        public void Column_name_configuration_on_non_root_type_is_propagated_to_derived_types()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro144459_Product>().Property(e => e.Name).HasColumnName("NameOfProduct");
            modelBuilder.Entity<Repro144459_Product>().ToTable("Products");
            modelBuilder.Entity<Repro144459_ClearanceProduct>().Map(mapping => { mapping.MapInheritedProperties(); }).
                ToTable("ClearanceProduct");
            modelBuilder.Entity<Repro144459_DiscontinuedProduct>().Map(mapping => { mapping.MapInheritedProperties(); })
                .ToTable("DiscontinuedProduct");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro144459_Product>("Products")
                .DbEqual(true, t => t.Columns.Any(c => c.Name == "NameOfProduct"));
            databaseMapping.Assert<Repro144459_ClearanceProduct>("ClearanceProduct")
                .DbEqual(true, t => t.Columns.Any(c => c.Name == "NameOfProduct"));
        }

        [Fact]
        public void Abstract_principal_can_be_mapped_with_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro110459_Order>();
            modelBuilder.Entity<Repro110459_Customer>();

            modelBuilder.Entity<Repro110459_Account>()
                .Map(mc => { mc.MapInheritedProperties(); })
                .ToTable("Accounts");

            modelBuilder.Entity<Repro110459_Contact>()
                .Map(mc => { mc.MapInheritedProperties(); })
                .ToTable("Contacts");

            modelBuilder.Entity<Repro110459_Order>().HasRequired(o => o.BillToCustomer).WithMany(c => c.Orders).
                HasForeignKey(o => o.BillToCustomerId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        #endregion

        #region Hybrid

        [Fact]
        public void Can_move_one_type_to_TPT_in_TPH_hierarchy()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPHRoot>().ToTable("Woof");
            modelBuilder.Entity<TPHNodeC>().ToTable("TableC");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TPHRoot>("Woof")
                .HasColumns("Id", "RootData", "AData", "BData", "Discriminator");
            databaseMapping.Assert<TPHNodeC>("TableC")
                .HasColumns("Id", "CData")
                .HasForeignKeyColumn("Id");
            databaseMapping.AssertMapping<TPHRoot>("Woof", true)
                .HasNoPropertyConditions()
                .HasNoColumnConditions();
            databaseMapping.AssertMapping<TPHRoot>("Woof", false)
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHRoot");
            databaseMapping.AssertMapping<TPHNodeA>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHNodeA");
            databaseMapping.AssertMapping<TPHNodeB>("Woof")
                .HasNoPropertyConditions()
                .HasColumnCondition("Discriminator", "TPHNodeB");
            databaseMapping.AssertMapping<TPHNodeC>("TableC")
                .HasNoPropertyConditions()
                .HasNoColumnConditions();
        }

        [Fact]
        public void Mix_TPH_and_TPT_by_mapping_one_middle_type_to_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>().ToTable("Base");
            modelBuilder.Entity<HybridL1A>();
            modelBuilder.Entity<HybridL1B>().ToTable("L1B");
            modelBuilder.Entity<HybridL2>().ToTable("L2");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<HybridBase>("Base")
                .HasColumns("Id", "BaseData", "L1AData", "Discriminator");
            databaseMapping.AssertMapping<HybridBase>("Base", false)
                .HasColumnCondition("Discriminator", "HybridBase");
            databaseMapping.Assert<HybridL1A>("Base")
                .HasColumns("Id", "BaseData", "L1AData", "Discriminator");
            databaseMapping.AssertMapping<HybridL1A>("Base")
                .HasColumnCondition("Discriminator", "HybridL1A");

            databaseMapping.Assert<HybridL1B>("L1B")
                .HasColumns("Id", "L1BData");
            databaseMapping.AssertMapping<HybridL1B>("L1B")
                .HasNoColumnConditions();

            databaseMapping.Assert<HybridL2>("L2")
                .HasColumns("Id", "L2Data");
            databaseMapping.AssertMapping<HybridL2>("L2")
                .HasNoColumnConditions();
        }

        [Fact]
        public void Mix_TPH_and_TPT_by_mapping_one_middle_type_to_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>().ToTable("Base");
            modelBuilder.Entity<HybridL1A>().ToTable("A");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<HybridBase>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridBase>("Base", false)
                .HasColumnCondition("Discriminator", "HybridBase");

            databaseMapping.Assert<HybridL1A>("A")
                .HasColumns("Id", "L1AData");
            databaseMapping.AssertMapping<HybridL1A>("A")
                .HasNoColumnConditions();

            databaseMapping.Assert<HybridL1B>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL1B>("Base")
                .HasColumnCondition("Discriminator", "HybridL1B");

            databaseMapping.Assert<HybridL2>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL2>("Base")
                .HasColumnCondition("Discriminator", "HybridL2");
        }

        [Fact]
        public void Mix_TPH_and_TPT_by_mapping_one_middle_type_to_TPT_with_derived_type_configuration()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>().ToTable("Base");
            modelBuilder.Entity<HybridL1A>().ToTable("A");
            modelBuilder.Entity<HybridL2>().Map(mc => { });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<HybridBase>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridBase>("Base", false)
                .HasColumnCondition("Discriminator", "HybridBase");

            databaseMapping.Assert<HybridL1A>("A")
                .HasColumns("Id", "L1AData");
            databaseMapping.AssertMapping<HybridL1A>("A")
                .HasNoColumnConditions();

            databaseMapping.Assert<HybridL1B>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL1B>("Base")
                .HasColumnCondition("Discriminator", "HybridL1B");

            databaseMapping.Assert<HybridL2>("Base")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL2>("Base")
                .HasColumnCondition("Discriminator", "HybridL2");
        }

        [Fact]
        public void Mix_TPH_and_TPT_by_mapping_one_middle_type_to_TPT_using_attribute()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro143351_A1>()
                .Map(mapping => { mapping.Requires("luqkau").HasValue("1"); })
                .Map<Repro143351_A3>(mapping => { mapping.Requires("luqkau").HasValue("2"); })
                .Map<Repro143351_A4>(mapping => { mapping.Requires("luqkau").HasValue("3"); })
                .ToTable("A1");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Mix_TPH_and_TPC_by_mapping_one_middle_type_to_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>();
            modelBuilder.Entity<HybridL1A>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("L1");
                     });
            modelBuilder.Entity<HybridL1B>();
            modelBuilder.Entity<HybridL2>();

            Assert.Throws<NotSupportedException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mix_TPH_and_TPC_by_mapping_both_middle_types_to_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>();
            modelBuilder.Entity<HybridL1A>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("L1A");
                     });
            modelBuilder.Entity<HybridL1B>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("L1B");
                     });
            modelBuilder.Entity<HybridL2>();

            Assert.Throws<NotSupportedException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mix_TPH_and_TPC_and_TPT_by_mapping_one_middle_type_to_TPC_and_one_middle_type_to_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>();
            modelBuilder.Entity<HybridL1A>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("L1A");
                     });
            modelBuilder.Entity<HybridL1B>()
                .Map(mc => { mc.ToTable("L1B"); });
            modelBuilder.Entity<HybridL2>();

            Assert.Throws<NotSupportedException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mix_TPH_and_TPT_by_mapping_middle_type_to_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>();
            modelBuilder.Entity<HybridL1A>()
                .Map(mc => { mc.ToTable("L1"); });
            modelBuilder.Entity<HybridL1B>();
            modelBuilder.Entity<HybridL2>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<HybridBase>("HybridBases")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridBase>("HybridBases", true)
                .HasNoColumnConditions();
            databaseMapping.AssertMapping<HybridBase>("HybridBases", false)
                .HasColumnCondition("Discriminator", "HybridBase");
            databaseMapping.Assert<HybridL1A>("L1")
                .HasColumns("Id", "L1AData");
            databaseMapping.AssertMapping<HybridL1A>("L1")
                .HasNoColumnConditions();
            databaseMapping.Assert<HybridL1B>("HybridBases")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL1B>("HybridBases")
                .HasColumnCondition("Discriminator", "HybridL1B");
            databaseMapping.Assert<HybridL2>("HybridBases")
                .HasColumns("Id", "BaseData", "L2Data", "L1BData", "Discriminator");
            databaseMapping.AssertMapping<HybridL2>("HybridBases")
                .HasColumnCondition("Discriminator", "HybridL2");
        }

        [Fact]
        public void Can_alternate_abstract_in_TPH_hierarchy()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<ACrazy>();
            modelBuilder.Entity<BCrazy>();
            modelBuilder.Entity<CCrazy>();
            modelBuilder.Entity<DCrazy>();
            modelBuilder.Entity<ECrazy>();
            modelBuilder.Ignore<XCrazy>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<ACrazy>("ACrazies")
                .HasColumns("Id", "A", "B", "C", "D", "E", "Discriminator");

            databaseMapping.AssertMapping<ACrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "ACrazy");

            databaseMapping.AssertNoMapping<BCrazy>();

            databaseMapping.AssertMapping<CCrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "CCrazy");

            databaseMapping.AssertNoMapping<DCrazy>();

            databaseMapping.AssertMapping<ECrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "ECrazy");
        }

        [Fact]
        public void Can_alternate_abstract_in_TPH_hierarchy_with_extra_TPT_leaf()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<ACrazy>();
            modelBuilder.Entity<BCrazy>();
            modelBuilder.Entity<CCrazy>();
            modelBuilder.Entity<DCrazy>();
            modelBuilder.Entity<ECrazy>();
            modelBuilder.Entity<XCrazy>().ToTable("Tx");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<ACrazy>("ACrazies")
                .HasColumns("Id", "A", "B", "C", "D", "E", "Discriminator");

            databaseMapping.Assert<XCrazy>("Tx")
                .HasColumns("Id", "X");

            databaseMapping.AssertMapping<ACrazy>("ACrazies", true)
                .HasNoColumnConditions();
            databaseMapping.AssertMapping<ACrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "ACrazy");

            databaseMapping.AssertNoMapping<BCrazy>();

            databaseMapping.AssertMapping<CCrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "CCrazy");

            databaseMapping.AssertNoMapping<DCrazy>();

            databaseMapping.AssertMapping<ECrazy>("ACrazies", false)
                .HasColumnCondition("Discriminator", "ECrazy");

            databaseMapping.AssertMapping<XCrazy>("Tx", false)
                .HasNoColumnConditions();
        }

        [Fact]
        // Regression for 142318
        public void Can_have_TPH_alone_at_base_of_3_level_heirarchy()
        {
            //         E1       -- TPH alone
            //       / | \
            //      E2 E3 E4    -- all TPC
            //     /
            //    E5            -- TPT to E2
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Entity1>().Map(mapping =>
                                               {
                                                   mapping.ToTable("Entity1");
                                                   mapping.Requires(
                                                       "aduvbkqcgjqomqpyniiftbsqzzeehyhyddqlbdtmhvmdnoktgjbylbqumovbhn")
                                                       .HasValue("true").HasColumnType("varchar(max)");
                                               });
            modelBuilder.Entity<Entity4>().Map(mapping =>
                                               {
                                                   mapping.ToTable("Entity4");
                                                   mapping.MapInheritedProperties();
                                               });
            modelBuilder.Entity<Entity2>().Map(mapping =>
                                               {
                                                   mapping.ToTable("Entity2");
                                                   mapping.MapInheritedProperties();
                                               });
            modelBuilder.Entity<Entity5>().Map(mapping =>
                                               {
                                                   mapping.ToTable("Entity5");
                                                   mapping.Requires("rdfmyypkivldry").HasValue(-10).HasColumnType(
                                                       "bigint");
                                               });
            modelBuilder.Entity<Entity3>().Map(mapping =>
                                               {
                                                   mapping.ToTable("Entity3");
                                                   mapping.MapInheritedProperties();
                                               });
            modelBuilder.Entity<Entity1>().Property(p => p.Entity1_Col1).HasColumnType("smallint");
            modelBuilder.Entity<Entity1>().Property(p => p.Entity1_Col2).HasColumnType("datetime2");
            modelBuilder.Entity<Entity1>().Property(p => p.Entity1_Col3).HasColumnType("binary").HasMaxLength(10).
                IsFixedLength();
            modelBuilder.Entity<Entity1>().Property(p => p.Id).IsRequired().HasColumnType("char").HasMaxLength(20).
                IsFixedLength();
            modelBuilder.Entity<Entity2>().Property(p => p.Entity2_Col1).HasColumnType("smalldatetime");
            modelBuilder.Entity<Entity3>().Property(p => p.Entity3_Col1).IsRequired().HasColumnType("ntext").IsUnicode();
            modelBuilder.Entity<Entity3>().Property(p => p.Entity3_Col2).HasColumnType("datetimeoffset");
            modelBuilder.Entity<Entity4>().Property(p => p.Entity4_Col1).HasColumnType("bigint");
            modelBuilder.Entity<Entity4>().Property(p => p.Entity4_Col2).HasColumnType("varchar(max)");
            modelBuilder.Entity<Entity5>().Property(p => p.Entity5_Col1).HasColumnType("bit");
            modelBuilder.Entity<Entity5>().Property(p => p.Entity5_Col2).HasColumnType("smalldatetime");
            modelBuilder.Entity<Entity5>().Property(p => p.Entity5_Col3).HasColumnType("time");
            modelBuilder.Entity<Entity5>().Property(p => p.Entity5_Col4).HasColumnType("decimal").HasPrecision(28, 4);
            modelBuilder.Entity<Entity5>().Property(p => p.Entity5_Col5).HasColumnType("date");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Mixed_TPH_and_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro137329_A1>()
                .Map(mapping => { mapping.ToTable("A1"); })
                .Map<Repro137329_A3>(mapping => { mapping.ToTable("A3"); })
                .Map<Repro137329_A4>(mapping => { mapping.ToTable("A4"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Mixed_TPH_and_TPC_with_TPC_has_nullable_discriminator()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro137329_A1>().Map(mapping => { mapping.ToTable("A1"); }).Map<Repro137329_A3>(
                mapping =>
                {
                    mapping.ToTable("A3");
                    mapping.MapInheritedProperties();
                }).Map<Repro137329_A4>(mapping =>
                                       {
                                           mapping.ToTable("A4");
                                           mapping.MapInheritedProperties();
                                       });

            Assert.Throws<NotSupportedException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mixed_TPH_and_TPC_and_TPT_has_nullable_discriminator()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro137329_A1>().Map(mapping => { mapping.ToTable("A1"); }).Map<Repro137329_A3>(
                mapping =>
                {
                    mapping.ToTable("A3");
                    mapping.MapInheritedProperties();
                }).Map<Repro137329_A4>(mapping => { mapping.ToTable("A4"); });

            Assert.Throws<NotSupportedException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void TPC_under_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<HybridBase>().ToTable("T1");
            modelBuilder.Entity<HybridL1A>().ToTable("T2");
            modelBuilder.Entity<HybridL1B>().ToTable("T3");
            modelBuilder.Entity<HybridL2>().Map(mapping =>
                                                {
                                                    mapping.ToTable("T4");
                                                    mapping.MapInheritedProperties();
                                                });

            Assert.Throws<NotSupportedException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        #endregion

        #region Entity Splitting

        [Fact]
        public void Setting_all_Properties_does_not_trigger_entity_splitting()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name, a.BaseData });
                         mc.ToTable("Tbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(1, databaseMapping.Database.Schemas.Single().Tables.Count);
        }

        [Fact]
        public void Entity_split_single_type_including_pks()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("NameTbl")
                .HasColumns("Id", "Name")
                .DbEqual(DbStoreGeneratedPattern.Identity,
                         t => t.Columns.Single(c => c.Name == "Id").StoreGeneratedPattern);
            databaseMapping.Assert<AssocBase>("DataTbl")
                .HasColumns("Id", "BaseData")
                .HasForeignKeyColumn("Id", "NameTbl")
                .DbEqual(DbStoreGeneratedPattern.None, t => t.Columns.Single(c => c.Name == "Id").StoreGeneratedPattern);
        }

        [Fact]
        public void Entity_split_single_type_without_specifying_table_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            Assert.Throws<InvalidOperationException>(() =>
                                                     modelBuilder.Entity<AssocBase>()
                                                         .Map(mc => mc.Properties(a => new { a.Id, a.Name }))
                                                         .Map(mc =>
                                                              {
                                                                  mc.Properties(a => new { a.Id, a.BaseData });
                                                                  mc.ToTable("DataTbl");
                                                              }));
        }

        [Fact]
        public void Entity_split_single_type_not_including_pks()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2, databaseMapping.Database.Schemas.Single().Tables.Count);
            Assert.Equal(2,
                         databaseMapping.EntityContainerMappings[0].EntitySetMappings[0].EntityTypeMappings[0].
                             TypeMappingFragments.Count);
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
        }

        [Fact]
        public void Empty_properties_call_in_middle_makes_extra_fragments()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { });
                         mc.ToTable("Empty");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(3, databaseMapping.Database.Schemas.Single().Tables.Count);
            Assert.Equal(3,
                         databaseMapping.EntityContainerMappings[0].EntitySetMappings[0].EntityTypeMappings[0].
                             TypeMappingFragments.Count);
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
            databaseMapping.Assert<AssocBase>("Empty").HasColumns("Id");
        }

        [Fact]
        public void Mapping_property_twice_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData, a.Name });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mapping_subtype_twice_chained_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map<AssocDerived>(mc => { });

            Assert.Throws<InvalidOperationException>(() =>
                                                     modelBuilder.Entity<AssocBase>()
                                                         .Map<AssocDerived>(mc => { }));

            modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        public void Mapping_subtype_twice_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map<AssocDerived>(mc => mc.ToTable("derived"));

            modelBuilder.Entity<AssocDerived>()
                .Map(mc => mc.ToTable("deriveds"));

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mapping_property_again_after_everything_mapped_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name, a.BaseData });
                         mc.ToTable("AssocBase");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("Other");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mapping_inherited_properties_after_everything_mapped_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name, a.BaseData });
                         mc.ToTable("AssocBase");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("Other");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mapping_all_properties_after_everything_mapped_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc => mc.ToTable("AssocBase"))
                .Map(mc => mc.ToTable("Other"));
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Mapping_inherited_properties_twice_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("One");
                     })
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Two");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(b => b.AssocRelatedId);

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Empty_properties_call_at_end_makes_extra_fragments()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { });
                         mc.ToTable("Empty");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(3, databaseMapping.Database.Schemas.Single().Tables.Count);
            Assert.Equal(3,
                         databaseMapping.EntityContainerMappings[0].EntitySetMappings[0].EntityTypeMappings[0].
                             TypeMappingFragments.Count);
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
            databaseMapping.Assert<AssocBase>("Empty").HasColumns("Id");
        }

        [Fact]
        public void Properties_with_only_PK_creates_extra_fragment()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id });
                         mc.ToTable("Empty");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Ignore<AssocDerived>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
            databaseMapping.Assert<AssocBase>("Empty").HasColumns("Id");
        }

        [Fact]
        public void Entity_split_derived_type_with_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.DerivedData1 });
                         mc.ToTable("Deriveds1");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.DerivedData2 });
                         mc.ToTable("Deriveds2");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>().HasColumns("Id", "Name", "BaseData");
            databaseMapping.Assert<AssocDerived>("Deriveds1").HasColumns("Id", "DerivedData1");
            databaseMapping.Assert<AssocDerived>("Deriveds2").HasColumns("Id", "DerivedData2");
        }

        [Fact]
        public void Entity_split_base_type_with_derived_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocDerived>().ToTable("Deriveds");
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocDerived>().HasColumns("Id", "DerivedData1", "DerivedData2");
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
        }

        [Fact]
        public void Entity_split_derived_type_with_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>().ToTable("Bases");
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.DerivedData1 });
                         mc.ToTable("Deriveds1");
                     })
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.Properties(a => new { a.Id, a.DerivedData2 });
                         mc.ToTable("Deriveds2");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocBase>().HasColumns("Id", "Name", "BaseData");
            databaseMapping.Assert<AssocDerived>("Deriveds1").HasColumns("Id", "DerivedData1");
            databaseMapping.Assert<AssocDerived>("Deriveds2").HasColumns("Id", "Name", "BaseData", "DerivedData2");
        }

        [Fact]
        public void Entity_split_base_type_with_derived_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AssocBase>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.Name });
                         mc.ToTable("NameTbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.Id, a.BaseData });
                         mc.ToTable("DataTbl");
                     });
            modelBuilder.Entity<AssocDerived>()
                .Map(mc =>
                     {
                         mc.MapInheritedProperties();
                         mc.ToTable("Deriveds");
                     });
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBase);
            modelBuilder.Entity<AssocBase>().Ignore(b => b.AssocRelatedBaseId);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelated);
            modelBuilder.Entity<AssocDerived>().Ignore(d => d.AssocRelatedId);

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AssocDerived>().HasColumns("Id", "Name", "BaseData", "DerivedData1", "DerivedData2");
            databaseMapping.Assert<AssocBase>("NameTbl").HasColumns("Id", "Name");
            databaseMapping.Assert<AssocBase>("DataTbl").HasColumns("Id", "BaseData");
        }

        [Fact]
        public void Entity_split_base_type_with_derived_TPT_with_abstract_middle_type()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro147822_EntityA>().Map(mapping =>
                                                           {
                                                               mapping.ToTable("Table1");
                                                               mapping.Properties(e => new
                                                                                       {
                                                                                           e.Name,
                                                                                           e.Description,
                                                                                       });
                                                           });
            modelBuilder.Entity<Repro147822_EntityA>().Map(mapping =>
                                                           {
                                                               mapping.ToTable("Table2");
                                                               mapping.Properties(e => e.Photo);
                                                           });
            modelBuilder.Entity<Repro147822_EntityB>().Map(mapping => { mapping.ToTable("Table3"); });
            modelBuilder.Entity<Repro147822_EntityC>().Map(mapping => { mapping.ToTable("Table4"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro147822_EntityA>("Table1")
                .HasColumns("Id", "Description", "Name");
            databaseMapping.Assert<Repro147822_EntityA>("Table2")
                .HasColumns("Id", "Photo")
                .HasForeignKeyColumn("Id", "Table1");
            databaseMapping.Assert<Repro147822_EntityB>("Table3")
                .HasColumns("Id", "Details")
                .HasForeignKeyColumn("Id", "Table1");
            databaseMapping.Assert<Repro147822_EntityC>("Table4")
                .HasColumns("Id", "Color")
                .HasForeignKeyColumn("Id", "Table3");
        }

        [Fact]
        public void Entity_split_base_type_with_derived_TPH_with_abstract_base_with_duplicate_property_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro147906_EntityA1>().Map(mapping => { mapping.ToTable("Table1"); });
            modelBuilder.Entity<Repro147906_EntityA1_1>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table1");
                                                                  mapping.Requires("discriminator").HasValue("Entity1_1");
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table1");
                                                                  mapping.Properties(e => e.Property4);
                                                                  mapping.Requires("discriminator").HasValue("Entity1_2");
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table2");
                                                                  mapping.Properties(e => e.Property6);
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table3");
                                                                  mapping.Properties(e => e.Property6);
                                                              });

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Entity_split_base_type_with_derived_TPH_with_abstract_base()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro147906_EntityA1>().Map(mapping => { mapping.ToTable("Table1"); });
            modelBuilder.Entity<Repro147906_EntityA1_1>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table1");
                                                                  mapping.Requires("discriminator").HasValue("Entity1_1");
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table1");
                                                                  mapping.Properties(e => e.Property4);
                                                                  mapping.Requires("discriminator").HasValue("Entity1_2");
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table2");
                                                                  mapping.Properties(e => e.Property5);
                                                              });
            modelBuilder.Entity<Repro147906_EntityA1_2>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table3");
                                                                  mapping.Properties(e => e.Property6);
                                                              });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro147906_EntityA1_1>("Table1")
                .HasColumns("Id", "Photo", "Description", "Name", "Property1", "Property2", "Property3", "Property4",
                            "discriminator");
            databaseMapping.Assert<Repro147906_EntityA1_2>("Table1")
                .HasColumns("Id", "Photo", "Description", "Name", "Property1", "Property2", "Property3", "Property4",
                            "discriminator");
            databaseMapping.Assert<Repro147906_EntityA1_2>("Table2")
                .HasColumns("Id", "Property5");
            databaseMapping.Assert<Repro147906_EntityA1_2>("Table3")
                .HasColumns("Id", "Property6");
        }

        [Fact]
        public void Entity_split_base_type_with_a_complex_type_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro148415_EntityC1>().Map(mapping =>
                                                            {
                                                                mapping.ToTable("Table1");
                                                                mapping.Requires("d").HasValue("EntityC1");
                                                                mapping.Properties(e => e.Property2);
                                                                mapping.Properties(e => e.ComplexProperty1.P2);
                                                            });
            modelBuilder.Entity<Repro148415_EntityC1>().Map(mapping =>
                                                            {
                                                                mapping.ToTable("Table2");
                                                                mapping.Properties(e => e.ComplexProperty1.P1);
                                                            });
            modelBuilder.Entity<Repro148415_EntityC1_1>().Map(mapping =>
                                                              {
                                                                  mapping.ToTable("Table1");
                                                                  mapping.Requires("d").HasValue("EntityC1_1");
                                                              });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<Repro148415_EntityC1>("Table1")
                .HasColumns("Id", "ComplexProperty1_P2", "Property2", "Property3", "Property4", "d");

            databaseMapping.Assert<Repro148415_EntityC1>("Table2")
                .HasColumns("Id", "ComplexProperty1_P1");

            databaseMapping.AssertMapping<Repro148415_EntityC1>("Table1", false)
                .HasColumnCondition("d", "EntityC1");

            databaseMapping.AssertMapping<Repro148415_EntityC1>("Table1", true)
                .HasNoColumnConditions();

            databaseMapping.Assert<Repro148415_EntityC1_1>("Table1")
                .HasColumns("Id", "ComplexProperty1_P2", "Property2", "Property3", "Property4", "d");

            databaseMapping.AssertMapping<Repro148415_EntityC1_1>("Table1", false)
                .HasColumnCondition("d", "EntityC1_1");
        }

        [Fact]
        public void Entity_split_middle_type_in_TPH()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro147929_EntityB1>().Map(mapping =>
                                                       {
                                                           mapping.ToTable("Table1");
                                                           mapping.Requires("d").HasValue("EntityB1");
                                                       });
            builder.Entity<Repro147929_EntityB1_1>().Map(mapping =>
                                                         {
                                                             mapping.ToTable("Table1");
                                                             mapping.Properties(e => e.Property1);
                                                             mapping.Requires("d").HasValue("EntityB1_1");
                                                         });
            builder.Entity<Repro147929_EntityB1_1>().Map(mapping =>
                                                         {
                                                             mapping.ToTable("Table2");
                                                             mapping.Properties(e => new { e.Property2, e.Property3 });
                                                         });
            builder.Entity<Repro147929_EntityB1_1_1>().Map(mapping =>
                                                           {
                                                               mapping.ToTable("Table1");
                                                               mapping.Requires("d").HasValue("EntityB1_1_1");
                                                           });

            builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo, true);
        }

        [Fact]
        public void Entity_split_middle_type_in_TPH2()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<Repro147929_EntityB1>().Map(mapping =>
                                                       {
                                                           mapping.ToTable("Table1");
                                                           mapping.Requires("d").HasValue("EntityB1");
                                                       });
            builder.Entity<Repro147929_EntityB1_1>().Map(mapping =>
                                                         {
                                                             mapping.ToTable("Table1");
                                                             mapping.Properties(e => e.Property1);
                                                             mapping.Requires("d").HasValue("EntityB1_1");
                                                         });
            builder.Entity<Repro147929_EntityB1_1>().Map(mapping =>
                                                         {
                                                             mapping.ToTable("Table2");
                                                             mapping.Properties(e => new { e.Property2, e.Property3 });
                                                         });
            builder.Entity<Repro147929_EntityB1_1_1>().Map(mapping =>
                                                           {
                                                               mapping.ToTable("Table1");
                                                               mapping.Requires("d").HasValue("EntityB1_1_1");
                                                           });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Entity_splitting_maintains_configured_composite_key_ordering()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<EntityWithCompositePK>().Map(mapping =>
                                                        {
                                                            mapping.ToTable("Table1");
                                                            mapping.Properties(e => e.Property1);
                                                        });
            builder.Entity<EntityWithCompositePK>().Map(mapping =>
                                                        {
                                                            mapping.ToTable("Table2");
                                                            mapping.Properties(e => e.Property2);
                                                        });
            builder.Entity<EntityWithCompositePK>().HasKey(e => new
                                                                {
                                                                    e.Key2,
                                                                    e.Key1,
                                                                });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<EntityWithCompositePK>("Table1")
                .HasColumns("Key1", "Key2", "Property1");
            databaseMapping.Assert<EntityWithCompositePK>("Table2")
                .HasColumns("Key1", "Key2", "Property2")
                .HasForeignKey(new string[] { "Key1", "Key2" }, "Table1");
        }

        [Fact]
        public void Entity_splitting_derived_table_creates_FK_to_first_table()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<EntityL>().Map(mapping => { mapping.ToTable("Table1"); });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table2");
                                                mapping.Properties(e => new { e.Property1, e.Property2 });
                                                mapping.Properties(e => e.Property3);
                                            });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table3");
                                                mapping.Properties(e => e.Property4);
                                            });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<EntityL>("Table1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table2")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table3")
                .HasForeignKeyColumn("Id", "Table2");
        }

        [Fact]
        public void Entity_splitting_derived_TPC_creates_FK_to_first_table()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<EntityL>().Map(mapping => { mapping.ToTable("Table1"); });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table2");
                                                mapping.Properties(e => new { e.Property1, e.Property3 });
                                            });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table3");
                                                mapping.Properties(e => new { e.Property2, e.Property4 });
                                            });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<EntityL>("Table1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table2")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table3")
                .HasForeignKeyColumn("Id", "Table2");
        }

        [Fact]
        public void Entity_splitting_all_inherited_props_in_derived_TPC_creates_FK_to_first_table()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<EntityL>().Map(mapping => { mapping.ToTable("Table1"); });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table2");
                                                mapping.Properties(e => new { e.Property3, e.Property4 });
                                            });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table3");
                                                mapping.Properties(e => new { e.Property1, e.Property2 });
                                            });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<EntityL>("Table1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table2")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table3")
                .HasForeignKeyColumn("Id", "Table2");
        }

        [Fact]
        public void Entity_splitting_derived_TPT_creates_FK_to_first_table()
        {
            var builder = new AdventureWorksModelBuilder();

            builder.Entity<EntityL>().Map(mapping => { mapping.ToTable("Table1"); });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table2");
                                                mapping.Properties(e => new { e.Property3 });
                                            });
            builder.Entity<EntityL_1>().Map(mapping =>
                                            {
                                                mapping.ToTable("Table3");
                                                mapping.Properties(e => new { e.Property4 });
                                            });

            var databaseMapping = builder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);

            databaseMapping.Assert<EntityL>("Table1")
                .HasNoForeignKeyColumns();

            databaseMapping.Assert<EntityL_1>("Table2")
                .HasForeignKeyColumn("Id", "Table1")
                .DbEqual(1, t => t.ForeignKeyConstraints.Count);

            databaseMapping.Assert<EntityL_1>("Table3")
                .HasForeignKeyColumn("Id", "Table2")
                .DbEqual(1, t => t.ForeignKeyConstraints.Count);
        }

        [Fact]
        public void Entity_splitting_names_store_entity_types_and_sets_properly()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TypeClass>()
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.IntProp });
                         mc.ToTable("Tbl");
                     })
                .Map(mc =>
                     {
                         mc.Properties(a => new { a.StringProp });
                         mc.ToTable("Tbl2");
                     }).Map(mc =>
                            {
                                mc.Properties(a => new { a.ByteArrayProp, a.DateTimeProp, a.DecimalProp });
                                mc.ToTable("Tbl3");
                            });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            var mws = databaseMapping.ToMetadataWorkspace();

            var itemCollection = mws.GetItemCollection(DataSpace.SSpace) as StoreItemCollection;

            Assert.NotNull(itemCollection.GetItem<EntityType>("CodeFirstDatabaseSchema.TypeClass"));
            Assert.NotNull(itemCollection.GetItem<EntityType>("CodeFirstDatabaseSchema.TypeClass1"));
            Assert.NotNull(itemCollection.GetItem<EntityType>("CodeFirstDatabaseSchema.TypeClass2"));
        }

        #endregion

        #region Table Splitting

        [Fact]
        public void One_to_one_relationship_can_share_table()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailDiffKey);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItemDetail>().ToTable("Items");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetail).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(3, t => t.Columns.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(0, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TSItem>("Items").HasColumns("Id", "Name", "Detail");
        }

        [Fact]
        // Regression test for Dev 11 bug 86852
        public void Table_split_with_conflicting_key_column_order_throws()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailDiffKey);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItem>().HasKey(c => new { c.Id, c.Name });
            modelBuilder.Entity<TSItem>().Property(i => i.Id).HasColumnOrder(1);
            modelBuilder.Entity<TSItem>().Property(i => i.Name).HasColumnOrder(2);
            modelBuilder.Entity<TSItemDetail>().ToTable("Items");
            modelBuilder.Entity<TSItemDetail>().HasKey(c => new { c.Id, c.Detail });
            modelBuilder.Entity<TSItemDetail>().Property(i => i.Id).HasColumnOrder(2);
            modelBuilder.Entity<TSItemDetail>().Property(i => i.Detail).HasColumnOrder(1);
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetail).WithRequiredPrincipal();

            Assert.Throws<InvalidOperationException>(() =>
                                                     modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Table_split_with_conflicting_primary_key_columnType_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailDiffKey);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItem>().Property(i => i.Id).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<TSItemDetail>().ToTable("Items");
            modelBuilder.Entity<TSItemDetail>().Property(i => i.Id).HasColumnType("ntext");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetail).WithRequiredPrincipal();

            Assert.Throws<InvalidOperationException>(() => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo));
        }

        [Fact]
        public void Table_split_with_different_primary_key_name_uses_principal_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetail);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItemDetailDiffKey>().HasKey(x => x.DiffId);
            modelBuilder.Entity<TSItemDetailDiffKey>().ToTable("Items");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetailDiffKey).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(3, t => t.Columns.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(0, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TSItem>("Items").HasColumns("Id", "Name", "Detail");
        }

        [Fact]
        public void Table_split_with_different_primary_key_property_name_that_is_mapped_to_same_column_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetail);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItemDetailDiffKey>().HasKey(x => x.DiffId);
            modelBuilder.Entity<TSItemDetailDiffKey>().Property(x => x.DiffId).HasColumnName("Id");
            modelBuilder.Entity<TSItemDetailDiffKey>().ToTable("Items");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetailDiffKey).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(3, t => t.Columns.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(0, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TSItem>("Items").HasColumns("Id", "Name", "Detail");
        }

        [Fact]
        public void Table_split_with_different_primary_key_property_name_that_is_mapped_to_new_column_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetail);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailOverlappingProperty);
            modelBuilder.Entity<TSItemDetailDiffKey>().HasKey(x => x.DiffId);
            modelBuilder.Entity<TSItemDetailDiffKey>().Property(x => x.DiffId).HasColumnName("Foo");
            modelBuilder.Entity<TSItemDetailDiffKey>().ToTable("Items");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetailDiffKey).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(3, t => t.Columns.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(0, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TSItem>("Items").HasColumns("Foo", "Name", "Detail");
        }

        [Fact]
        public void Table_split_with_overlapping_property_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetail);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailDiffKey);
            modelBuilder.Entity<TSItemDetailOverlappingProperty>().ToTable("Items");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetailOverlappingProperty).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            databaseMapping.Assert<TSItem>("Items").HasColumns("Id", "Name", "Name1");
        }

        [Fact]
        public void Table_split_with_overlapping_property_name_that_has_been_renamed()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSItem>().ToTable("Items");
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetail);
            modelBuilder.Entity<TSItem>().Ignore(x => x.TSItemDetailDiffKey);
            modelBuilder.Entity<TSItemDetailOverlappingProperty>().ToTable("Items");
            modelBuilder.Entity<TSItemDetailOverlappingProperty>().Property(x => x.Name).HasColumnName("OtherName");
            modelBuilder.Entity<TSItem>().HasRequired(i => i.TSItemDetailOverlappingProperty).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(2, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(3, t => t.Columns.Count);
            databaseMapping.Assert<TSItem>("Items").DbEqual(0, t => t.ForeignKeyConstraints.Count);
            databaseMapping.Assert<TSItem>("Items").HasColumns("Id", "Name", "OtherName");
        }

        [Fact]
        public void Table_split_with_IA_one_to_one_relationship_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSIAItem>().ToTable("Items");
            modelBuilder.Entity<TSIAItemDetail>().ToTable("Items");
            modelBuilder.Entity<TSIAItem>().HasRequired(i => i.Detail).WithRequiredPrincipal();

            Assert.Equal(Strings.EntityMappingConfiguration_InvalidTableSharing("TSIAItemDetail", "TSIAItem", "Items"),
                         Assert.Throws<InvalidOperationException>(
                             () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo)).Message);
        }

        [Fact]
        public void Table_split_base_on_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSBase>().Map(mc =>
                                              {
                                                  mc.ToTable("TS");
                                                  mc.Requires("disc").HasValue("TSBase");
                                              });
            modelBuilder.Entity<TSDerived>().Map(mc =>
                                                 {
                                                     mc.ToTable("TS");
                                                     mc.Requires("disc").HasValue("TSDerived");
                                                 });
            modelBuilder.Entity<TSDerived>().Ignore(x => x.Detail);
            modelBuilder.Entity<TSBaseDetail>().Map(mc => { mc.ToTable("TS"); });
            modelBuilder.Entity<TSBase>().HasRequired(i => i.BaseDetail).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            databaseMapping.Assert<TSBase>("TS").HasColumns("Id", "BaseData", "DerivedData", "disc", "BaseDetail");
            databaseMapping.Assert<TSDerived>("TS").HasColumns("Id", "BaseData", "DerivedData", "disc", "BaseDetail");
            databaseMapping.Assert<TSBaseDetail>("TS").HasColumns("Id", "BaseData", "DerivedData", "disc", "BaseDetail");
        }

        [Fact]
        public void Table_split_base_on_default_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSBase>();
            modelBuilder.Entity<TSDerived>();
            modelBuilder.Entity<TSDerived>().Ignore(x => x.Detail);
            modelBuilder.Entity<TSBaseDetail>().Map(mc => { mc.ToTable("TSBase"); });
            modelBuilder.Entity<TSBase>().HasRequired(i => i.BaseDetail).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            databaseMapping.Assert<TSBase>("TSBase").HasColumns("Id", "BaseData", "DerivedData", "Discriminator",
                                                                "BaseDetail");
            databaseMapping.Assert<TSDerived>("TSBase").HasColumns("Id", "BaseData", "DerivedData", "Discriminator",
                                                                   "BaseDetail");
            databaseMapping.Assert<TSBaseDetail>("TSBase").HasColumns("Id", "BaseData", "DerivedData", "Discriminator",
                                                                      "BaseDetail");
        }

        [Fact]
        public void Table_split_base_on_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSBase>().Map(mc => { mc.ToTable("A"); });
            modelBuilder.Entity<TSBaseDetail>().Map(mc => { mc.ToTable("A"); });
            modelBuilder.Entity<TSDerived>().Map(mc => { mc.ToTable("B"); });
            modelBuilder.Entity<TSDerived>().Ignore(x => x.Detail);
            modelBuilder.Entity<TSBase>().HasRequired(i => i.BaseDetail).WithRequiredPrincipal();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            databaseMapping.Assert<TSBase>("A").HasColumns("Id", "BaseData", "BaseDetail");
            databaseMapping.Assert<TSDerived>("B").HasColumns("Id", "DerivedData");
            databaseMapping.Assert<TSBaseDetail>("A").HasColumns("Id", "BaseData", "BaseDetail");
        }

        [Fact]
        public void Table_split_base_on_TPT_2()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro140107_Vehicle>().Map(mapping => { mapping.ToTable("Vehicle"); });
            modelBuilder.Entity<Repro140107_Car>().Map(mapping => { mapping.ToTable("Car"); });
            modelBuilder.Entity<Repro140107_Model>().Map(mapping => { mapping.ToTable("Vehicle"); });
            modelBuilder.Entity<Repro140107_Vehicle>().HasRequired(e => e.Repro140107_Model).WithRequiredPrincipal(
                e => e.Repro140107_Vehicle);
            modelBuilder.Entity<Repro140107_Model>().HasRequired(e => e.Repro140107_Vehicle).WithRequiredDependent(
                e => e.Repro140107_Model);
            modelBuilder.Entity<Repro140107_Vehicle>().Property(p => p.Repro140107_VehicleId).HasColumnType("int");
            modelBuilder.Entity<Repro140107_Vehicle>().Property(p => p.Name).HasColumnType("smallint");
            modelBuilder.Entity<Repro140107_Car>().Property(p => p.Type).HasColumnType("bigint");
            modelBuilder.Entity<Repro140107_Model>().Property(p => p.Repro140107_ModelId).HasColumnType("int");
            modelBuilder.Entity<Repro140107_Model>().Property(p => p.Description).HasColumnType("bit");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
        }

        [Fact]
        // Doesn't work due to an EF bug
        public void Table_split_base_on_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TSBase>().Map(mc => { mc.ToTable("A"); });
            modelBuilder.Entity<TSBaseDetail>().Map(mc => { mc.ToTable("A"); });
            modelBuilder.Entity<TSDerived>().Map(mc =>
                                                 {
                                                     mc.MapInheritedProperties();
                                                     mc.ToTable("B");
                                                 });
            modelBuilder.Entity<TSDerived>().Ignore(x => x.Detail);

            modelBuilder.Entity<TSBase>().HasRequired(i => i.BaseDetail).WithRequiredPrincipal();

            Assert.Throws<MappingException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo, true));
        }

        [Fact]
        // Doesn't work due to an EF bug
        public void Table_split_base_on_TPC_2()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Repro140106_Vehicle>().Map(mapping => { mapping.ToTable("Vehicle"); });
            modelBuilder.Entity<Repro140106_Car>().Map(mapping =>
                                                       {
                                                           mapping.ToTable("Car");
                                                           mapping.MapInheritedProperties();
                                                       });
            modelBuilder.Entity<Repro140106_Model>().ToTable("Vehicle");
            modelBuilder.Entity<Repro140106_Vehicle>().HasRequired(e => e.Model).WithRequiredPrincipal(
                e => e.Repro140106_Vehicle);
            modelBuilder.Entity<Repro140106_Model>().HasRequired(e => e.Repro140106_Vehicle).WithRequiredDependent(
                e => e.Model);
            modelBuilder.Entity<Repro140106_Vehicle>().Property(p => p.Repro140106_VehicleId).IsRequired().HasColumnType
                ("nvarchar").HasMaxLength(40).IsUnicode().IsVariableLength();
            modelBuilder.Entity<Repro140106_Vehicle>().Property(p => p.Name).HasColumnType("nvarchar(max)").IsUnicode();
            modelBuilder.Entity<Repro140106_Car>().Property(p => p.Type).IsRequired().HasColumnType("text");
            modelBuilder.Entity<Repro140106_Model>().Property(p => p.Repro140106_ModelId).IsRequired().HasColumnType(
                "nvarchar").HasMaxLength(40).IsUnicode().IsVariableLength();
            modelBuilder.Entity<Repro140106_Model>().Property(p => p.Description).HasColumnType("numeric").HasPrecision(
                15, 5);

            Assert.Throws<MappingException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo, true));
        }

        [Fact]
        // Doesn't work due to an EF bug
        public void Table_split_base_on_TPC_has_no_FK_to_base_and_table_split_entity_props_not_inherited()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TPCVehicle>().Map(mapping => { mapping.ToTable("Vehicle"); });
            modelBuilder.Entity<TPCCar>().Map(mapping =>
                                              {
                                                  mapping.ToTable("Car");
                                                  mapping.MapInheritedProperties();
                                              });
            modelBuilder.Entity<TPCModel>().ToTable("Vehicle");
            modelBuilder.Entity<TPCVehicle>().HasRequired(e => e.TPCModel).WithRequiredPrincipal(e => e.TPCVehicle);
            modelBuilder.Entity<TPCModel>().HasRequired(e => e.TPCVehicle).WithRequiredDependent(e => e.TPCModel);
            modelBuilder.Entity<TPCVehicle>().Property(p => p.TPCVehicleId).IsRequired().HasColumnType("nvarchar").
                HasMaxLength(40).IsUnicode().IsVariableLength();
            modelBuilder.Entity<TPCVehicle>().Property(p => p.Name).HasColumnType("nvarchar(max)").IsUnicode();
            modelBuilder.Entity<TPCCar>().Property(p => p.Type).IsRequired().HasColumnType("text");
            modelBuilder.Entity<TPCModel>().Property(p => p.TPCModelId).IsRequired().HasColumnType("nvarchar").
                HasMaxLength(40).IsUnicode().IsVariableLength();
            modelBuilder.Entity<TPCModel>().Property(p => p.Description).HasColumnType("numeric").HasPrecision(15, 5);

            Assert.Throws<MappingException>(
                () => modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo, true));
        }

        #endregion

        #region Complex Types

        [Fact]
        public void ComplexType_Properties_Can_Be_Entity_Split()
        {
            var modelBuilder = new AdventureWorksModelBuilder();
            modelBuilder.Ignore<CTRegion>();
            modelBuilder.ComplexType<CTAddress>().Ignore(c => c.Region);

            modelBuilder.Entity<CTBase>()
                .Map(mc =>
                     {
                         mc.Properties(p => new { p.Id, p.HomeAddress });
                         mc.ToTable("Home");
                     })
                .Map(mc =>
                     {
                         mc.Properties(p => new { p.Id, p.WorkAddress });
                         mc.ToTable("Work");
                     });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<CTBase>("Home").HasColumns("Id", "HomeAddress_Street", "HomeAddress_City");
            databaseMapping.Assert<CTBase>("Work").HasColumns("Id", "WorkAddress_Street", "WorkAddress_City");
        }

        [Fact]
        public void Scalars_Inside_ComplexType_Properties_Can_Be_Entity_Split()
        {
            var modelBuilder = new AdventureWorksModelBuilder();
            modelBuilder.Ignore<CTRegion>();
            modelBuilder.Entity<CTBase>().Ignore(b => b.WorkAddress);
            modelBuilder.ComplexType<CTAddress>().Ignore(c => c.Region);

            modelBuilder.Entity<CTBase>()
                .Map(mc =>
                     {
                         mc.Properties(p => new { p.Id, p.HomeAddress.Street });
                         mc.ToTable("HomeStreet");
                     })
                .Map(mc =>
                     {
                         mc.Properties(p => new { p.Id, p.HomeAddress.City });
                         mc.ToTable("HomeCity");
                     });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<CTBase>("HomeStreet").HasColumns("Id", "HomeAddress_Street");
            databaseMapping.Assert<CTBase>("HomeCity").HasColumns("Id", "HomeAddress_City");
        }

        #endregion

        #region Abstract In Middle

        [Fact]
        public void Abstract_in_middle_of_hierarchy_with_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsInMiddle>();
            modelBuilder.Entity<AbsInMiddleL1>();
            modelBuilder.Entity<AbsInMiddleL2>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AbsInMiddle>("AbsInMiddles").HasColumns("Id", "Data", "L1Data", "L2Data",
                                                                           "Discriminator");
            databaseMapping.AssertNoMapping<AbsInMiddleL1>();
            databaseMapping.Assert<AbsInMiddleL2>("AbsInMiddles").HasColumns("Id", "Data", "L1Data", "L2Data",
                                                                             "Discriminator");
        }

        [Fact]
        public void Abstract_in_middle_of_hierarchy_with_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsInMiddle>().ToTable("Base");
            modelBuilder.Entity<AbsInMiddleL1>().ToTable("L1");
            modelBuilder.Entity<AbsInMiddleL2>().ToTable("L2");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.Assert<AbsInMiddle>("Base").HasColumns("Id", "Data");
            databaseMapping.Assert<AbsInMiddleL1>("L1").HasColumns("Id", "L1Data");
            databaseMapping.Assert<AbsInMiddleL2>("L2").HasColumns("Id", "L2Data");
        }

        [Fact]
        public void Abstract_in_middle_of_hierarchy_with_TPC()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsInMiddle>().ToTable("Base");
            modelBuilder.Entity<AbsInMiddleL2>(
                ).Map(mc =>
                      {
                          mc.ToTable("L2");
                          mc.MapInheritedProperties();
                      });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            Assert.Equal(2, databaseMapping.Database.Schemas.Single().Tables.Count());
            databaseMapping.Assert<AbsInMiddle>("Base").HasColumns("Id", "Data");
            databaseMapping.Assert<AbsInMiddleL2>("L2").HasColumns("Id", "Data", "L1Data", "L2Data");
        }

        [Fact]
        public void Entity_split_abstract_middle_in_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<EntityN>().Map(mapping => { mapping.ToTable("Table1"); });
            modelBuilder.Entity<EntityN_1>().Map(mapping =>
                                                 {
                                                     mapping.ToTable("Table2");
                                                     mapping.Properties(e => new
                                                                             {
                                                                                 e.Property2,
                                                                             });
                                                 });
            modelBuilder.Entity<EntityN_1>().Map(mapping =>
                                                 {
                                                     mapping.ToTable("Table3");
                                                     mapping.Properties(e => new
                                                                             {
                                                                                 e.Property2_1,
                                                                             });
                                                 });
            modelBuilder.Entity<EntityN_1_1>().Map(mapping => { mapping.ToTable("Table4"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }


        [Fact]
        public void Entity_split_abstract_middle_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<EntityN>().Map(mapping => { mapping.ToTable("Table1"); });
            modelBuilder.Entity<EntityN_1>().Map(mapping =>
                                                 {
                                                     mapping.ToTable("Table1");
                                                     mapping.Properties(e => new
                                                                             {
                                                                                 e.Id,
                                                                                 e.Property1,
                                                                                 e.Property1_1,
                                                                                 e.Property2,
                                                                             });
                                                 });
            modelBuilder.Entity<EntityN_1>().Map(mapping =>
                                                 {
                                                     mapping.ToTable("Table2");
                                                     mapping.Properties(e => new
                                                                             {
                                                                                 e.Id,
                                                                                 e.Property2_1,
                                                                             });
                                                 });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        #endregion

        #region Abstract At Base

        [Fact]
        public void Can_specify_not_null_condition_on_derived_with_abstract_base_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBase>();
            modelBuilder.Entity<AbsAtBaseL1>().Map(mc => { mc.Requires(e => e.L1Data).HasValue(); });
            modelBuilder.Entity<AbsAtBaseL1>().Property(p => p.L1Data).IsRequired();
            modelBuilder.Ignore<AbsAtBaseL2>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.AssertMapping<AbsAtBase>("AbsAtBases", true).HasNoColumnConditions();
            databaseMapping.AssertMapping<AbsAtBaseL1>("AbsAtBases").HasNullabilityColumnCondition("L1Data", false);
        }

        [Fact]
        public void Can_specify_not_null_condition_on_derived_and_null_value_on_base_with_abstract_base_in_TPH_throws()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsAtBase>().Map(mc => { mc.Requires("L1Data").HasValue(null); });
            modelBuilder.Entity<AbsAtBaseL1>().Map(mc => { mc.Requires(e => e.L1Data).HasValue(); });
            modelBuilder.Entity<AbsAtBaseL1>().Property(p => p.L1Data).IsRequired();
            modelBuilder.Ignore<AbsAtBaseL2>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);
            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
            databaseMapping.AssertMapping<AbsAtBase>("AbsAtBases").HasNoColumnConditions();
            databaseMapping.AssertMapping<AbsAtBaseL1>("AbsAtBases").HasNullabilityColumnCondition("L1Data", false);
        }

        [Fact]
        public void Entity_split_abstract_base_in_TPT()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsBaseSplit>().Map(mc =>
                                                    {
                                                        mc.ToTable("Base1");
                                                        mc.Properties(b => new { b.Id, b.Prop1 });
                                                    }).Map(mc =>
                                                           {
                                                               mc.ToTable("Base2");
                                                               mc.Properties(b => new { b.Id, b.Prop2 });
                                                           });

            modelBuilder.Entity<AbsDerivedSplit>().Map(mc => { mc.ToTable("Derived"); });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        [Fact]
        public void Entity_split_abstract_base_in_TPH()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<AbsBaseSplit>().Map(mc =>
                                                    {
                                                        mc.ToTable("Base1");
                                                        mc.Properties(b => new { b.Id, b.Prop1 });
                                                    }).Map(mc =>
                                                           {
                                                               mc.ToTable("Base2");
                                                               mc.Properties(b => new { b.Id, b.Prop2 });
                                                           });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.Equal(1, databaseMapping.EntityContainerMappings.Single().EntitySetMappings.Count);
        }

        #endregion
    }
}