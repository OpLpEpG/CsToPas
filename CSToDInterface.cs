using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using RGiesecke.DllExport;
using System.Collections;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Security.AccessControl;
using System.Windows.Markup;

namespace XMLScemaToDelphi
{
    [ComImport, Guid("4B538C5E-DC77-437C-893B-97488A82E509"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlQualifiedName
    {

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsEmpty();


        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();


        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Namespace();


        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ToString();

        [return: MarshalAs(UnmanagedType.Interface)]
        object XmlObject();

    }
    public class TXmlQualifiedName : IXmlQualifiedName
    {
        private readonly XmlQualifiedName qn;

        public TXmlQualifiedName(XmlQualifiedName qn) => this.qn = qn;
        bool IXmlQualifiedName.IsEmpty() => qn.IsEmpty;
        string IXmlQualifiedName.Name() => qn.Name;
        string IXmlQualifiedName.Namespace() => qn.Namespace;
        string IXmlQualifiedName.ToString() => qn.ToString();
        object IXmlQualifiedName.XmlObject() => qn;

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlQualifiedName(XmlQualifiedName en, out IXmlQualifiedName OutD)
        {
            OutD = null;
            try
            {
                OutD = new TXmlQualifiedName(en);
            }
            catch (Exception e)
            {
                MessageBox.Show("EXCEPTION is " + e.Message);
            }
        }

    }
    //
    // Сводка:
    //     Представляет корневой класс для иерархии модели объектов схемы Xml и служит базовым
    //     классом для классов, таких как System.Xml.Schema.XmlSchema класса.
    [ComImport, Guid("AF1853D2-C560-4E24-A15B-5518F030F4FC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaObject
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object XmlObject();

        int LineNumber();
        int LinePosition();
        //
        // Сводка:
        //     Возвращает или задает исходное расположение для файла, загрузившего данную схему.
        //
        // Возврат:
        //     Исходное расположение (URI) для файла.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string SourceUri();
        //
        // Сводка:
        //     Возвращает или задает родительский элемент объекта System.Xml.Schema.XmlSchemaObject.
        //
        // Возврат:
        //     Родительский System.Xml.Schema.XmlSchemaObject данного System.Xml.Schema.XmlSchemaObject.
        IXmlSchemaObject Parent();
        //
        // Сводка:
        //     Возвращает или задает System.Xml.Serialization.XmlSerializerNamespaces для использования
        //     с этим объектом схемы.
        //
        // Возврат:
        //     System.Xml.Serialization.XmlSerializerNamespaces Свойство для объекта схемы.

        //XmlSerializerNamespaces Namespaces { get; set; }
        void GetNamespaces([MarshalAs(UnmanagedType.LPArray)] out XmlQualifiedName[] Namespaces, out int Count);
    }
    public abstract class TXmlSchemaObject : IXmlSchemaObject
    {
        public readonly XmlSchemaObject x;
        public TXmlSchemaObject(XmlSchemaObject x) => this.x = x;
        void IXmlSchemaObject.GetNamespaces(out XmlQualifiedName[] Namespaces, out int Count)
        {
            Namespaces = x.Namespaces.ToArray();
            Count = Namespaces.Length;
        }
        int IXmlSchemaObject.LineNumber() => x.LineNumber;
        int IXmlSchemaObject.LinePosition() => x.LinePosition;
        IXmlSchemaObject IXmlSchemaObject.Parent() => GetXmlSchemaObject(x.Parent);
        string IXmlSchemaObject.SourceUri() => x.SourceUri;
        object IXmlSchemaObject.XmlObject() => x;
        public static IXmlSchemaObject GetXmlSchemaObject(XmlSchemaObject x)
        {
            if (x is XmlSchema) return new TXmlSchema(x as XmlSchema);
            else if (x is XmlSchemaSimpleType) return new TXmlSchemaSimpleType(x as XmlSchemaSimpleType);
            else if (x is XmlSchemaComplexType) return new TXmlSchemaComplexType(x as XmlSchemaComplexType);
            else if (x is XmlSchemaElement) return new TXmlSchemaElement(x as XmlSchemaElement);
            else if (x is XmlSchemaAttribute) return new TXmlSchemaAttribute(x as XmlSchemaAttribute);
            else if (x is XmlSchemaImport) return new TXmlSchemaImport(x as XmlSchemaImport);
            else if (x is XmlSchemaInclude) return new TXmlSchemaInclude(x as XmlSchemaInclude);            
            else if (x is XmlSchemaGroup) return new TXmlSchemaGroup(x as XmlSchemaGroup);
            else if (x is XmlSchemaAttributeGroup) return new TXmlSchemaAttributeGroup(x as XmlSchemaAttributeGroup);
            else if (x is XmlSchemaNotation) return new TXmlSchemaNotation(x as XmlSchemaNotation);
            else if (x is XmlSchemaAnnotation) return new TXmlSchemaAnnotation(x as XmlSchemaAnnotation);
            else if (x is XmlSchemaParticle) return TXmlSchemaParticle.GetPaticle(x as XmlSchemaParticle) as IXmlSchemaObject;
            else return null;
        }
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaObject(XmlSchemaObject a, out IXmlSchemaObject OutD) => OutD = GetXmlSchemaObject(a);
    }

    [ComImport, Guid("41A3E555-6A86-4F71-A211-3674292D0B4F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAnnotated
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAnnotation();
    }
    public abstract class TXmlSchemaAnnotated : TXmlSchemaObject, IXmlSchemaObject, IXmlSchemaAnnotated
    {
        public TXmlSchemaAnnotated(XmlSchemaAnnotated x) : base(x) { }
        string IXmlSchemaAnnotated.GetAnnotation()
        {
            return GetDoc((x as XmlSchemaAnnotated).Annotation);
        }
        public static string GetDoc(XmlSchemaAnnotation ann)
        {
            string s = "";

            if (ann == null) return s;

            foreach (XmlSchemaObject o in ann.Items)
            {
                XmlNode[] d = null;
                if (o is XmlSchemaAppInfo) d = (o as XmlSchemaAppInfo).Markup;
                else if (o is XmlSchemaDocumentation) d = (o as XmlSchemaDocumentation).Markup;

                foreach (XmlNode n in d) s += n.Value + "\n\n";
            }
            return s;
        }
    }

    [ComImport, Guid("BA1CE72D-2BA7-4F27-9C8A-CCBB43D20102"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXMLEnumeraror
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetCurrent();


        [return: MarshalAs(UnmanagedType.U1)]
        bool MoveNext();


        void Reset();
    }
    public class TXMLEnumerator : IXMLEnumeraror
    {
        private readonly IEnumerator e;

        public TXMLEnumerator(IEnumerator e) => this.e = e;
        object IXMLEnumeraror.GetCurrent() => e.Current;
        bool IXMLEnumeraror.MoveNext() => e.MoveNext();
        void IXMLEnumeraror.Reset() => e.Reset();
        //object ICsObject.Csharp() => e;

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXMLEnumeraror(IEnumerable en, out IXMLEnumeraror OutD)
        {
            OutD = null;
            try
            {
                OutD = new TXMLEnumerator(en.GetEnumerator());
            }
            catch (Exception e)
            {
                MessageBox.Show("EXCEPTION is " + e.Message);
            }
        }
    }

    [ComImport, Guid("33E5C30E-F668-4F17-B697-9ACF882ECA94"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXMLEnumerable
    {
        IXMLEnumeraror GetEnumerator();
    }
    public class TXMLEnumerable : IXMLEnumerable
    {
        private readonly IEnumerable e;

        public TXMLEnumerable(IEnumerable e) => this.e = e;

        IXMLEnumeraror IXMLEnumerable.GetEnumerator() => new TXMLEnumerator(e.GetEnumerator());

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXMLEnumerable(IEnumerable en, out IXMLEnumerable OutD)
        {
            OutD = null;
            try
            {
                OutD = new TXMLEnumerable(en);
            }
            catch (Exception e)
            {
                MessageBox.Show("EXCEPTION is " + e.Message);
            }
        }

    }

    [ComImport, Guid("8CE6A050-4442-4EDD-986A-F3F5897040DE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaObjectTable
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        XmlSchemaObject GetItem(XmlQualifiedName QName);
        int Count();
        IXMLEnumerable Names();
        IXMLEnumerable Values();
        bool Contains(XmlQualifiedName name);
    }
    public class TXmlSchemaObjectTable : IXmlSchemaObjectTable, IXMLEnumerable
    {
        private readonly XmlSchemaObjectTable ot;

        public TXmlSchemaObjectTable(XmlSchemaObjectTable ot) => this.ot = ot;
        XmlSchemaObject IXmlSchemaObjectTable.GetItem(XmlQualifiedName QName) => ot[QName];
        int IXmlSchemaObjectTable.Count() => ot.Count;
        IXMLEnumerable IXmlSchemaObjectTable.Names() => new TXMLEnumerable(ot.Names);
        IXMLEnumerable IXmlSchemaObjectTable.Values() => new TXMLEnumerable(ot.Values);
        bool IXmlSchemaObjectTable.Contains(XmlQualifiedName name) => ot.Contains(name);
        IXMLEnumeraror IXMLEnumerable.GetEnumerator() => new TXMLEnumerator(ot.Values.GetEnumerator());
    }

    [ComImport, Guid("29854D89-BD09-4627-B8E7-C648DA4131A9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaObjectCollection 
    {
        int Count();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetItem(int index);
    }
    public class TXmlSchemaObjectCollection : IXmlSchemaObjectCollection, IXMLEnumerable
    {
        private readonly XmlSchemaObjectCollection c;
        public TXmlSchemaObjectCollection(XmlSchemaObjectCollection c) => this.c = c;
        IXMLEnumeraror IXMLEnumerable.GetEnumerator() => new TXMLEnumerator(c.GetEnumerator());
        int IXmlSchemaObjectCollection.Count() => c.Count;
        object IXmlSchemaObjectCollection.GetItem(int index) => c[index];
    }
/// <summary>
/// Schema
/// </summary>
    [ComImport, Guid("28C0864D-D3E0-4C44-8912-05161629BCA2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchema : IXmlSchemaObject
    {
        //
        // Сводка:
        //     Gets or sets the Uniform Resource Identifier (URI) of the schema target namespace.
        //
        // Возврат:
        //     The schema target namespace.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string TargetNamespace();
        //
        // Сводка:
        //     Gets the post-schema-compilation value of all schema types in the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectCollection of all schema types in the schema.
        IXmlSchemaObjectTable SchemaTypes();
        //
        // Сводка:
        //     Gets the post-schema-compilation value for all notations in the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectTable collection of all notations in the
        //     schema.
        IXmlSchemaObjectTable Notations();
        //
        // Сводка:
        //     Gets the collection of schema elements in the schema and is used to add new element
        //     types at the schema element level.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectCollection of schema elements in the schema.
        //[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
        //[XmlElement("attribute", typeof(XmlSchemaAttribute))]
        //[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup))]
        //[XmlElement("complexType", typeof(XmlSchemaComplexType))]
        //[XmlElement("element", typeof(XmlSchemaElement))]
        //[XmlElement("group", typeof(XmlSchemaGroup))]
        //[XmlElement("notation", typeof(XmlSchemaNotation))]
        //[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        IXmlSchemaObjectCollection Items();

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsCompiled();
        //
        // Сводка:
        //     Gets the collection of included and imported schemas.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectCollection of the included and imported schemas.
        IXmlSchemaObjectCollection Includes();
        string Id();
        //
        // Сводка:
        //     Gets the post-schema-compilation value of all the groups in the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectTable collection of all the groups in the
        //     schema.
        IXmlSchemaObjectTable Groups();
        //
        // Сводка:
        //     Gets or sets the finalDefault attribute which sets the default value of the final
        //     attribute on elements and complex types in the target namespace of the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaDerivationMethod value representing the different
        //     methods for preventing derivation. The default value is XmlSchemaDerivationMethod.None.
        //[DefaultValue(XmlSchemaDerivationMethod.None)]
        //[XmlAttribute("finalDefault")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod FinalDefault();
        //
        // Сводка:
        //     Gets the post-schema-compilation value for all the elements in the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectTable collection of all the elements in the
        //     schema.
        IXmlSchemaObjectTable Elements();
        //
        // Сводка:
        //     Gets or sets the form for elements declared in the target namespace of the schema.
        //
        // Возврат:
        //     The System.Xml.Schema.XmlSchemaForm value that indicates if elements from the
        //     target namespace are required to be qualified with the namespace prefix. The
        //     default is System.Xml.Schema.XmlSchemaForm.None.
        //[DefaultValue(XmlSchemaForm.None)]
        //[XmlAttribute("elementFormDefault")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaForm ElementFormDefault();
        //
        // Сводка:
        //     Gets or sets the blockDefault attribute which sets the default value of the block
        //     attribute on element and complex types in the targetNamespace of the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaDerivationMethod value representing the different
        //     methods for preventing derivation. The default value is XmlSchemaDerivationMethod.None.
        //[DefaultValue(XmlSchemaDerivationMethod.None)]
        //[XmlAttribute("blockDefault")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod BlockDefault();
        //
        // Сводка:
        //     Gets the post-schema-compilation value for all the attributes in the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectTable collection of all the attributes in
        //     the schema.

        IXmlSchemaObjectTable Attributes();
        //
        // Сводка:
        //     Gets the post-schema-compilation value of all the global attribute groups in
        //     the schema.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaObjectTable collection of all the global attribute
        //     groups in the schema.

        IXmlSchemaObjectTable AttributeGroups();
        //
        // Сводка:
        //     Gets or sets the form for attributes declared in the target namespace of the
        //     schema.
        //
        // Возврат:
        //     The System.Xml.Schema.XmlSchemaForm value that indicates if attributes from the
        //     target namespace are required to be qualified with the namespace prefix. The
        //     default is System.Xml.Schema.XmlSchemaForm.None.
        //[DefaultValue(XmlSchemaForm.None)]
        //[XmlAttribute("attributeFormDefault")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaForm AttributeFormDefault();
        //
        // Сводка:
        //     Gets or sets the qualified attributes which do not belong to the schema target
        //     namespace.
        //
        // Возврат:
        //     An array of qualified System.Xml.XmlAttribute objects that do not belong to the
        //     schema target namespace.
        //public XmlAttribute[] UnhandledAttributes { get; set; }
        //
        // Сводка:
        //     Gets or sets the version of the schema.
        //
        // Возврат:
        //     The version of the schema. The default value is String.Empty.
        //public string Version { get; set; }

        //
        // Сводка:
        //     Reads an XML Schema from the supplied System.Xml.XmlReader.
        //
        // Параметры:
        //   reader:
        //     The XmlReader containing the XML Schema to read.
        //
        //   validationEventHandler:
        //     The validation event handler that receives information about the XML Schema syntax
        //     errors.
        //
        // Возврат:
        //     The System.Xml.Schema.XmlSchema object representing the XML Schema.
        //
        // Исключения:
        //   T:System.Xml.Schema.XmlSchemaException:
        //     An System.Xml.Schema.XmlSchemaException is raised if no System.Xml.Schema.ValidationEventHandler
        //     is specified.
        //public static XmlSchema Read(XmlReader reader, ValidationEventHandler validationEventHandler);
    }
    public class TXmlSchema : TXmlSchemaObject, IXmlSchemaObject, IXmlSchema
    {
        public readonly XmlSchema s;
        public TXmlSchema(XmlSchema s) : base(s) => this.s = s;
        string IXmlSchema.TargetNamespace() => s.TargetNamespace;
        IXmlSchemaObjectTable IXmlSchema.SchemaTypes() => new TXmlSchemaObjectTable(s.SchemaTypes);
        IXmlSchemaObjectTable IXmlSchema.Notations() => new TXmlSchemaObjectTable(s.Notations);
        IXmlSchemaObjectCollection IXmlSchema.Items() => new TXmlSchemaObjectCollection(s.Items);
        bool IXmlSchema.IsCompiled() => s.IsCompiled;
        IXmlSchemaObjectCollection IXmlSchema.Includes() => new TXmlSchemaObjectCollection(s.Includes);
        string IXmlSchema.Id() => s.Id;
        IXmlSchemaObjectTable IXmlSchema.Groups() => new TXmlSchemaObjectTable(s.Groups);
        XmlSchemaDerivationMethod IXmlSchema.FinalDefault() => s.FinalDefault;
        IXmlSchemaObjectTable IXmlSchema.Elements() => new TXmlSchemaObjectTable(s.Elements);
        XmlSchemaForm IXmlSchema.ElementFormDefault() => s.ElementFormDefault;
        XmlSchemaDerivationMethod IXmlSchema.BlockDefault() => s.BlockDefault;
        IXmlSchemaObjectTable IXmlSchema.Attributes() => new TXmlSchemaObjectTable(s.Attributes);
        IXmlSchemaObjectTable IXmlSchema.AttributeGroups() => new TXmlSchemaObjectTable(s.AttributeGroups);
        XmlSchemaForm IXmlSchema.AttributeFormDefault() => s.AttributeFormDefault;
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchema(XmlSchema s, out IXmlSchema OutD) => OutD = new TXmlSchema(s);
    }
    /// <summary>
    /// IXmlSchemaType
    /// </summary>
    [ComImport, Guid("0AC541BF-F7D6-4770-9D7A-21C9599D7A36"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaType
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaType BaseXmlSchemaType();


        [return: MarshalAs(UnmanagedType.U4)]
        XmlTokenizedType TokenizedType();
        //XmlTypeCode TypeCode();

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDatatypeVariety Variety();


        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod DerivedBy();


        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod Final();


        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod FinalResolved();


        [return: MarshalAs(UnmanagedType.U1)]
        bool IsMixed();


        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();


        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlQualifiedName QualifiedName();


        [return: MarshalAs(UnmanagedType.U4)]
        XmlTypeCode TypeCode();
    }
    public abstract class TXmlSchemaType : TXmlSchemaAnnotated, IXmlSchemaObject, IXmlSchemaAnnotated, IXmlSchemaType
    {
        public readonly XmlSchemaType t;
        public TXmlSchemaType(XmlSchemaType t) : base(t) => this.t = t;
        public static IXmlSchemaType GetXmlSchemaType(XmlSchemaType st)
        {
            if (st is XmlSchemaSimpleType) return new TXmlSchemaSimpleType(st as XmlSchemaSimpleType);
            else if (st is XmlSchemaComplexType) return new TXmlSchemaComplexType(st as XmlSchemaComplexType);
            else return null;
        }
        IXmlSchemaType IXmlSchemaType.BaseXmlSchemaType() => GetXmlSchemaType(t.BaseXmlSchemaType);
        XmlTokenizedType IXmlSchemaType.TokenizedType() => t.Datatype.TokenizedType;
        XmlSchemaDatatypeVariety IXmlSchemaType.Variety() => t.Datatype.Variety;
        XmlSchemaDerivationMethod IXmlSchemaType.DerivedBy() => t.DerivedBy;
        XmlSchemaDerivationMethod IXmlSchemaType.Final() => t.Final;
        XmlSchemaDerivationMethod IXmlSchemaType.FinalResolved() => t.FinalResolved;
        bool IXmlSchemaType.IsMixed() => t.IsMixed;
        string IXmlSchemaType.Name() => t.Name;
        IXmlQualifiedName IXmlSchemaType.QualifiedName() => new TXmlQualifiedName(t.QualifiedName);
        XmlTypeCode IXmlSchemaType.TypeCode() => t.TypeCode;
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaType(XmlSchemaType a, out IXmlSchemaType OutD)
        {
            if (a is XmlSchemaComplexType) OutD = new TXmlSchemaComplexType(a as XmlSchemaComplexType);
            else OutD = new TXmlSchemaSimpleType(a as XmlSchemaSimpleType);
        }
    }
    /// <summary>
    /// Complex Type
    /// </summary>
    [ComImport, Guid("878FCB24-B379-4D1C-97E6-C16283EACADA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaParticle
    {

        [return: MarshalAs(UnmanagedType.I4)]
        decimal MinOccurs();
        // Возврат:
        //     Максимальное количество раз, которое может встречаться фрагмент. Значение по
        //     умолчанию — 1.

        [return: MarshalAs(UnmanagedType.I4)]
        decimal MaxOccurs();
    }
    public abstract class TXmlSchemaParticle : TXmlSchemaAnnotated, IXmlSchemaParticle, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaParticle(XmlSchemaParticle p) : base(p) { }
        decimal IXmlSchemaParticle.MinOccurs() => (x as XmlSchemaParticle).MinOccurs;
        decimal IXmlSchemaParticle.MaxOccurs() => (x as XmlSchemaParticle).MaxOccurs;
        public static IXmlSchemaParticle GetPaticle(XmlSchemaParticle p)
        {
            if (p is XmlSchemaAll) return new TXmlSchemaAll(p as XmlSchemaAll);
            else if (p is XmlSchemaChoice) return new TXmlSchemaChoice(p as XmlSchemaChoice);
            else if (p is XmlSchemaSequence) return new TXmlSchemaSequence(p as XmlSchemaSequence);
            else if (p is XmlSchemaGroupRef) return new TXmlSchemaGroupRef(p as XmlSchemaGroupRef);
            else if (p is XmlSchemaAny) return new TXmlSchemaAny(p as XmlSchemaAny);
            else return null;
        }

    }

    [ComImport, Guid("13E48227-9074-4A5D-9BD2-7247278C55F3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaChoice
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaObjectCollection Items();
    }
    public class TXmlSchemaChoice : TXmlSchemaParticle, IXmlSchemaParticle, IXmlSchemaChoice, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaChoice(XmlSchemaChoice p) : base(p) { }
        IXmlSchemaObjectCollection IXmlSchemaChoice.Items() => new TXmlSchemaObjectCollection((x as XmlSchemaChoice).Items);
    }

    [ComImport, Guid("1C471A5B-F299-4C56-9150-D7EE6A131433"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSequence
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaObjectCollection Items();
    }
    public class TXmlSchemaSequence : TXmlSchemaParticle, IXmlSchemaParticle, IXmlSchemaSequence, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaSequence(XmlSchemaSequence p) : base(p) { }
        IXmlSchemaObjectCollection IXmlSchemaSequence.Items() => new TXmlSchemaObjectCollection((x as XmlSchemaSequence).Items);
    }

    [ComImport, Guid("90021AF2-A4DC-43C4-8E32-7C85CAEC877C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAll
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaObjectCollection Items();
    }
    public class TXmlSchemaAll : TXmlSchemaParticle, IXmlSchemaParticle, IXmlSchemaAll, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaAll(XmlSchemaAll p) : base(p) { }
        IXmlSchemaObjectCollection IXmlSchemaAll.Items() => new TXmlSchemaObjectCollection((x as XmlSchemaAll).Items);
    }

    [ComImport, Guid("AD8CFD50-221C-4A23-9561-DEAD635D8606"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaGroupRef
    {
        IXmlQualifiedName RefName();
        //
        // Сводка:
        //     Получает один из System.Xml.Schema.XmlSchemaChoice, System.Xml.Schema.XmlSchemaAll,
        //     или System.Xml.Schema.XmlSchemaSequence классы, которые содержит значение после
        //     компиляции Particle свойство.
        //
        // Возврат:
        //     Значение после компиляции Particle свойство, которое является одним из System.Xml.Schema.XmlSchemaChoice,
        //     System.Xml.Schema.XmlSchemaAll, или System.Xml.Schema.XmlSchemaSequence классы.         
        IXmlSchemaParticle Particle();
    }
    public class TXmlSchemaGroupRef : TXmlSchemaParticle, IXmlSchemaParticle, IXmlSchemaGroupRef, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaGroupRef(XmlSchemaGroupRef p) : base(p) { }
        IXmlQualifiedName IXmlSchemaGroupRef.RefName() => new TXmlQualifiedName((x as XmlSchemaGroupRef).RefName);
        IXmlSchemaParticle IXmlSchemaGroupRef.Particle() => GetPaticle((x as XmlSchemaGroupRef).Particle);
        //{
        //    XmlSchemaGroupRef g = (XmlSchemaGroupRef) x;
        //    if (g.Particle is XmlSchemaAll)
        //    {
        //        return new TXmlSchemaAll((XmlSchemaAll)g.Particle);
        //    }
        //    else if (g.Particle is XmlSchemaChoice)
        //    {
        //        return new TXmlSchemaChoice((XmlSchemaChoice)g.Particle);
        //    }
        //    else 
        //    {
        //        return new TXmlSchemaSequence((XmlSchemaSequence)g.Particle);
        //    }
        //}
    }

    [ComImport, Guid("C54F1BA1-804E-4617-B57D-F003702632B6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaComplexType
    {
        //
        // Сводка:
        //     Возвращает или задает сведения, определяющие complexType элемент может использоваться
        //     в экземпляре документа.
        //
        // Возврат:
        //     Если true, элемент не может использовать этот complexType напрямую и должен использовать
        //     сложный тип, являющийся производным от этого complexType элемент. Значение по
        //     умолчанию — false. Необязательно.

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsAbstract();
        //
        // Сводка:
        //     Возвращает или задает block атрибута.
        //
        // Возврат:
        //     block Атрибут для предотвращения сложного типа в указанном типе наследования.
        //     Значение по умолчанию — XmlSchemaDerivationMethod.None. Необязательно.

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod Block();
        //
        // Сводка:
        //     Возвращает или задает сведения, определяющие, имеет ли сложный тип модель смешанного
        //     содержимого (разметка в рамках содержимого).
        //
        // Возврат:
        //     true, если символьные данные могут появляться между дочерними элементами данного
        //     сложного типа; в противном случае — false. Значение по умолчанию — false. Необязательно.

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsMixed();
        //
        // Сводка:
        //     Возвращает или задает после компиляции System.Xml.Schema.XmlSchemaContentModel
        //     этого сложного типа.
        //
        // Возврат:
        //     Тип модели содержимого, который является одним из System.Xml.Schema.XmlSchemaSimpleContent
        //     или System.Xml.Schema.XmlSchemaComplexContent классы.
        //[XmlElement("complexContent", typeof(XmlSchemaComplexContent))]
        //[XmlElement("simpleContent", typeof(XmlSchemaSimpleContent))]

        [return: MarshalAs(UnmanagedType.U1)]
        bool SimpleContentModel();
        //
        // Сводка:
        //     Возвращает или задает тип компоновщика как один из System.Xml.Schema.XmlSchemaGroupRef,
        //     System.Xml.Schema.XmlSchemaChoice, System.Xml.Schema.XmlSchemaAll, или System.Xml.Schema.XmlSchemaSequence
        //     классы.
        //
        // Возврат:
        //     Тип компоновщика.
        // [XmlElement("choice", typeof(XmlSchemaChoice))]
        // [XmlElement("sequence", typeof(XmlSchemaSequence))]
        //[XmlElement("group", typeof(XmlSchemaGroupRef))]
        //[XmlElement("all", typeof(XmlSchemaAll))]
///        [return: MarshalAs(UnmanagedType.Interface)]
///        IXmlSchemaParticle Particle();
        //
        // Сводка:
        //     Возвращает или задает значение для System.Xml.Schema.XmlSchemaAnyAttribute компонент
        //     сложного типа.
        //
        // Возврат:
        //     System.Xml.Schema.XmlSchemaAnyAttribute Компонент сложного типа.
        //[XmlElement("anyAttribute")]
        //public XmlSchemaAnyAttribute AnyAttribute { get; set; }
        //
        // Сводка:
        //     Возвращает модель содержимого сложного типа, которая содержит значение после
        //     компиляции.
        //
        // Возврат:
        //     Значение модели содержимого для сложного типа после компиляции.

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaContentType ContentType();
        //
        // Сводка:
        //     Возвращает фрагмент, который содержит значение после компиляции для System.Xml.Schema.XmlSchemaComplexType.ContentType
        //     примитивов.
        //
        // Возврат:
        //     Примитив для типа содержимого. Значение после компиляции System.Xml.Schema.XmlSchemaComplexType.ContentType
        //     примитивов.
        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaParticle ContentTypeParticle();
        //
        // Сводка:
        //     Возвращает значение после компиляции типа в информационный набор после проверки
        //     схемы. Это значение указывает, каким образом обеспечивается соответствие типов
        //     при xsi:type используется в экземпляре документа.
        //
        // Возврат:
        //     Значение информационного набора после проверки схемы. Значение по умолчанию —
        //     BlockDefault значение на schema элемент.
        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod BlockResolved();
        //
        // Сводка:
        //     Возвращает коллекцию всех соответствующих атрибутов данного сложного типа и его
        //     базовых типов.
        //
        // Возврат:
        //     Коллекция всех атрибутов из данного сложного типа и его базовых типов. Значение
        //     после компиляции AttributeUses свойство.

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaObjectTable AttributeUses();
    }
    public class TXmlSchemaComplexType : TXmlSchemaType, IXmlSchemaType, IXmlSchemaComplexType, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaComplexType(XmlSchemaComplexType t) : base(t) { }

        bool IXmlSchemaComplexType.IsAbstract() => (x as XmlSchemaComplexType).IsAbstract;
        XmlSchemaDerivationMethod IXmlSchemaComplexType.Block() => (x as XmlSchemaComplexType).Block;
        bool IXmlSchemaComplexType.IsMixed() => (x as XmlSchemaComplexType).IsMixed;
        bool IXmlSchemaComplexType.SimpleContentModel() => (x as XmlSchemaComplexType).ContentModel is XmlSchemaComplexContent;
        //IXmlSchemaParticle IXmlSchemaComplexType.Particle() => TXmlSchemaParticle.GetPaticle((x as XmlSchemaComplexType).Particle);
        XmlSchemaContentType IXmlSchemaComplexType.ContentType() => (x as XmlSchemaComplexType).ContentType;
        IXmlSchemaParticle IXmlSchemaComplexType.ContentTypeParticle() => TXmlSchemaParticle.GetPaticle((x as XmlSchemaComplexType).ContentTypeParticle);
        XmlSchemaDerivationMethod IXmlSchemaComplexType.BlockResolved() => (x as XmlSchemaComplexType).BlockResolved;
        IXmlSchemaObjectTable IXmlSchemaComplexType.AttributeUses() => new TXmlSchemaObjectTable((x as XmlSchemaComplexType).AttributeUses);
    }
/// <summary>
/// Simple Type
/// </summary>
    [ComImport, Guid("53BB52C5-0D13-4BD8-A500-4B73D3384B3A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSimpleTypeContent { };
    [ComImport, Guid("1AD7CCFB-0F43-4524-87BF-443A1A4BF36F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSimpleType
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaSimpleTypeContent Content();
    }
    public class TXmlSchemaSimpleType : TXmlSchemaType, IXmlSchemaType, IXmlSchemaSimpleType
    {
        public TXmlSchemaSimpleType(XmlSchemaSimpleType t) : base(t) { }
        IXmlSchemaSimpleTypeContent IXmlSchemaSimpleType.Content()
        {
            XmlSchemaSimpleType s = x as XmlSchemaSimpleType;
            if (s.Content is XmlSchemaSimpleTypeUnion)
                return new TXmlSchemaSimpleTypeUnion(s.Content as XmlSchemaSimpleTypeUnion);
            else if (s.Content is XmlSchemaSimpleTypeRestriction)
                return new TXmlSchemaSimpleTypeRestriction(s.Content as XmlSchemaSimpleTypeRestriction);
            else if (s.Content is XmlSchemaSimpleTypeList)
                return new TXmlSchemaSimpleTypeList(s.Content as XmlSchemaSimpleTypeList);
            else return null;
        }
    }
    [ComImport, Guid("848EB345-57C8-49E3-A1CC-FB8788DDB1FD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSimpleTypeUnion
    {

        [return: MarshalAs(UnmanagedType.U4)]
        int Count();

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaSimpleType Item(int Index);
    }
    public class TXmlSchemaSimpleTypeUnion : TXmlSchemaAnnotated, IXmlSchemaSimpleTypeUnion, IXmlSchemaSimpleTypeContent
    {
        public readonly XmlSchemaSimpleTypeUnion u;
        public TXmlSchemaSimpleTypeUnion(XmlSchemaSimpleTypeUnion u) : base(u) => this.u = u;
        int IXmlSchemaSimpleTypeUnion.Count() => u.BaseMemberTypes.Length;
        IXmlSchemaSimpleType IXmlSchemaSimpleTypeUnion.Item(int Index) => new TXmlSchemaSimpleType(u.BaseMemberTypes[Index]);
    }

    [ComImport, Guid("4C823B54-2055-4542-B763-C8628F3033D3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaFacet
    {
        // Возврат:
        //     Значение атрибута.

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Value();
        //
        // Сводка:
        //     Возвращает или задает сведения о том, что данный аспект является фиксированным.
        //
        // Возврат:
        //     Если true, значение фиксированного; в противном случае — false. Значение по умолчанию
        //     — false. Необязательно.
        //[DefaultValue(false)]
        //[XmlAttribute("fixed")]

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsFixed();


        [return: MarshalAs(UnmanagedType.U4)]
        int FacetType();
    }
    public class TXmlSchemaFacet : TXmlSchemaAnnotated, IXmlSchemaFacet
    {
        public readonly XmlSchemaFacet f;
        public TXmlSchemaFacet(XmlSchemaFacet f) : base(f) => this.f = f;

        string IXmlSchemaFacet.Value() => f.Value;
        bool IXmlSchemaFacet.IsFixed() => f.IsFixed;
        int IXmlSchemaFacet.FacetType()
        {
            if (f is XmlSchemaPatternFacet) return 1;
            else if (f is XmlSchemaLengthFacet) return 2;
            else if (f is XmlSchemaMinLengthFacet) return 3;
            else if (f is XmlSchemaMaxLengthFacet) return 4;
            else if (f is XmlSchemaMinInclusiveFacet) return 5;
            else if (f is XmlSchemaEnumerationFacet) return 6;
            else if (f is XmlSchemaMaxInclusiveFacet) return 8;
            else if (f is XmlSchemaMaxExclusiveFacet) return 9;
            else if (f is XmlSchemaTotalDigitsFacet) return 10;
            else if (f is XmlSchemaMinExclusiveFacet) return 11;
            else if (f is XmlSchemaFractionDigitsFacet) return 12;
            else if (f is XmlSchemaWhiteSpaceFacet) return 13;
            else return 0;
        }
    }
    [ComImport, Guid("F421BE69-C385-4B48-B83C-6AE3B81F986C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSimpleTypeRestriction
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlQualifiedName BaseTypeName();

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaSimpleType BaseType();
        //
        // Сводка:
        //     Возвращает или задает аспект схемы Xml.
        //
        // Возврат:
        //     Один из следующих классов аспектов: System.Xml.Schema.XmlSchemaLengthFacet, System.Xml.Schema.XmlSchemaMinLengthFacet,
        //     System.Xml.Schema.XmlSchemaMaxLengthFacet, System.Xml.Schema.XmlSchemaPatternFacet,
        //     System.Xml.Schema.XmlSchemaEnumerationFacet, System.Xml.Schema.XmlSchemaMaxInclusiveFacet,
        //     System.Xml.Schema.XmlSchemaMaxExclusiveFacet, System.Xml.Schema.XmlSchemaMinInclusiveFacet,
        //     System.Xml.Schema.XmlSchemaMinExclusiveFacet, System.Xml.Schema.XmlSchemaFractionDigitsFacet,
        //     System.Xml.Schema.XmlSchemaTotalDigitsFacet, System.Xml.Schema.XmlSchemaWhiteSpaceFacet.
        //[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
        //[XmlElement("length", typeof(XmlSchemaLengthFacet))]
        //[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
        //[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
        //[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
        //[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
        //[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
        //[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
        //[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
        //[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
        //[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
        //[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaObjectCollection Facets();
    }
    public class TXmlSchemaSimpleTypeRestriction : TXmlSchemaAnnotated, IXmlSchemaSimpleTypeRestriction, IXmlSchemaSimpleTypeContent
    {
        public readonly XmlSchemaSimpleTypeRestriction r;
        public TXmlSchemaSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction r) : base(r) => this.r = r;
        IXmlQualifiedName IXmlSchemaSimpleTypeRestriction.BaseTypeName() => new TXmlQualifiedName(r.BaseTypeName);
        IXmlSchemaSimpleType IXmlSchemaSimpleTypeRestriction.BaseType() => new TXmlSchemaSimpleType(r.BaseType);
        IXmlSchemaObjectCollection IXmlSchemaSimpleTypeRestriction.Facets() => new TXmlSchemaObjectCollection(r.Facets);
    }

    [ComImport, Guid("BFF8FD7F-14BE-4351-8882-0A5B47FAFA84"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSimpleTypeList
    {
        //
        // Сводка:
        //     Возвращает или задает имя встроенного типа данных или simpleType элемента, определенного
        //     в этой схеме (или другой схеме, задаваемой указанным пространством имен).
        //
        // Возврат:
        //     Имя типа списка простых типов.
        //[XmlAttribute("itemType")]
        //IXmlQualifiedName ItemTypeName();
        //
        // Сводка:
        //     Возвращает или задает simpleType элемент, который является производным от типа,
        //     заданного базовым значением.
        //
        // Возврат:
        //     Тип элемента для элемента простого типа.
        //[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        //IXmlSchemaSimpleType ItemType();
        //
        // Сводка:
        //     Возвращает или задает System.Xml.Schema.XmlSchemaSimpleType представляет тип
        //     simpleType на основе элемента System.Xml.Schema.XmlSchemaSimpleTypeList.ItemType
        //     и System.Xml.Schema.XmlSchemaSimpleTypeList.ItemTypeName значений простого типа.
        //
        // Возврат:
        //     System.Xml.Schema.XmlSchemaSimpleType Представляет тип simpleType элемента.
        //[XmlIgnore]

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaSimpleType BaseItemType();
    }
    public class TXmlSchemaSimpleTypeList : TXmlSchemaAnnotated, IXmlSchemaSimpleTypeList, IXmlSchemaSimpleTypeContent
    {
        private readonly XmlSchemaSimpleTypeList l;
        public TXmlSchemaSimpleTypeList(XmlSchemaSimpleTypeList l) : base(l) => this.l = l;
        IXmlSchemaSimpleType IXmlSchemaSimpleTypeList.BaseItemType() => new TXmlSchemaSimpleType(l.BaseItemType);
    }
/// <summary>
/// Element
/// </summary>
    [ComImport, Guid("62FA146A-6681-4A24-9413-AA8BF4B21B43"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaElement
    {
        //
        // Сводка:
        //     Gets or sets the type of the element. This can either be a complex type or a
        //     simple type.
        //
        // Возврат:
        //     The type of the element.
        //[XmlElement("complexType", typeof(XmlSchemaComplexType))]
        //[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]

///        IXmlSchemaType SchemaType();
        //
        // Сводка:
        //     Gets or sets the reference name of an element declared in this schema (or another
        //     schema indicated by the specified namespace).
        //
        // Возврат:
        //     The reference name of the element.
        //[XmlAttribute("ref")]

        IXmlQualifiedName RefName();
        //
        // Сводка:
        //     Gets the actual qualified name for the given element.
        //
        // Возврат:
        //     The qualified name of the element. The post-compilation value of the QualifiedName
        //     property.
        // [XmlIgnore]

        IXmlQualifiedName QualifiedName();
        //
        // Сводка:
        //     Gets or sets the name of the element.
        //
        // Возврат:
        //     The name of the element. The default is String.Empty.
        //[DefaultValue("")]
        //[XmlAttribute("name")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        //
        // Сводка:
        //     Gets or sets information that indicates if xsi:nil can occur in the instance
        //     data. Indicates if an explicit nil value can be assigned to the element.
        //
        // Возврат:
        //     If nillable is true, this enables an instance of the element to have the nil
        //     attribute set to true. The nil attribute is defined as part of the XML Schema
        //     namespace for instances. The default is false. Optional.
        //[DefaultValue(false)]
        //[XmlAttribute("nillable")]

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsNillable();
        //
        // Сводка:
        //     Gets or sets information to indicate if the element can be used in an instance
        //     document.
        //
        // Возврат:
        //     If true, the element cannot appear in the instance document. The default is false.
        //     Optional.
        //[DefaultValue(false)]
        //[XmlAttribute("abstract")]

        [return: MarshalAs(UnmanagedType.U1)]
        bool IsAbstract();
        //
        // Сводка:
        //     Gets or sets the form for the element.
        //
        // Возврат:
        //     The form for the element. The default is the System.Xml.Schema.XmlSchema.ElementFormDefault
        //     value. Optional.
        //[DefaultValue(XmlSchemaForm.None)]
        //[XmlAttribute("form")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaForm Form();
        //
        // Сводка:
        //     Gets or sets the name of a built-in data type defined in this schema or another
        //     schema indicated by the specified namespace.
        //
        // Возврат:
        //     The name of the built-in data type.
        // [XmlAttribute("type")]

        IXmlQualifiedName SchemaTypeName();
        //
        // Сводка:
        //     Gets or sets the fixed value.
        //
        // Возврат:
        //     The fixed value that is predetermined and unchangeable. The default is a null
        //     reference. Optional.
        //[DefaultValue(null)]
        //[XmlAttribute("fixed")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string FixedValue();
        //
        // Сводка:
        //     Gets or sets the Final property to indicate that no further derivations are allowed.
        //
        // Возврат:
        //     The Final property. The default is XmlSchemaDerivationMethod.None. Optional.
        //[DefaultValue(XmlSchemaDerivationMethod.None)]
        //[XmlAttribute("final")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod Final();
        //
        // Сводка:
        //     Gets an System.Xml.Schema.XmlSchemaType object representing the type of the element
        //     based on the System.Xml.Schema.XmlSchemaElement.SchemaType or System.Xml.Schema.XmlSchemaElement.SchemaTypeName
        //     values of the element.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaType object.

        IXmlSchemaType ElementSchemaType();
        //
        // Сводка:
        //     Gets or sets the default value of the element if its content is a simple type
        //     or content of the element is textOnly.
        //
        // Возврат:
        //     The default value for the element. The default is a null reference. Optional.
        //[DefaultValue(null)]
        //[XmlAttribute("default")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DefaultValue();
        //
        // Сводка:
        //     Gets the collection of constraints on the element.
        //
        // Возврат:
        //     The collection of constraints.
        //[XmlElement("key", typeof(XmlSchemaKey))]
        //[XmlElement("keyref", typeof(XmlSchemaKeyref))]
        //[XmlElement("unique", typeof(XmlSchemaUnique))]
        //public XmlSchemaObjectCollection Constraints { get; }
        //
        // Сводка:
        //     Gets the post-compilation value of the Block property.
        //
        // Возврат:
        //     The post-compilation value of the Block property. The default is the BlockDefault
        //     value on the schema element.

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod BlockResolved();
        //
        // Сводка:
        //     Gets or sets a Block derivation.
        //
        // Возврат:
        //     The attribute used to block a type derivation. Default value is XmlSchemaDerivationMethod.None.
        //     Optional.
        //[DefaultValue(XmlSchemaDerivationMethod.None)]
        //[XmlAttribute("block")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod Block();
        //
        // Сводка:
        //     Gets the post-compilation value of the Final property.
        //
        // Возврат:
        //     The post-compilation value of the Final property. Default value is the FinalDefault
        //     value on the schema element.
        // [XmlIgnore]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaDerivationMethod FinalResolved();
        //
        // Сводка:
        //     Gets or sets the name of an element that is being substituted by this element.
        //
        // Возврат:
        //     The qualified name of an element that is being substituted by this element. Optional.
        // [XmlAttribute("substitutionGroup")]

        IXmlQualifiedName SubstitutionGroup();
    }  
    public class TXmlSchemaElement : TXmlSchemaParticle, IXmlSchemaParticle, IXmlSchemaElement, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        private readonly XmlSchemaElement e;
        public TXmlSchemaElement(XmlSchemaElement e) : base(e) => this.e = e;

        //IXmlSchemaType IXmlSchemaElement.SchemaType() => TXmlSchemaType.GetXmlSchemaType(e.SchemaType);
        IXmlQualifiedName IXmlSchemaElement.RefName() => new TXmlQualifiedName(e.RefName);
        IXmlQualifiedName IXmlSchemaElement.QualifiedName() => new TXmlQualifiedName(e.QualifiedName); // e.QualifiedName.Name == "AbstractObject"
        string IXmlSchemaElement.Name() => e.Name;
        bool IXmlSchemaElement.IsNillable() => e.IsNillable;
        bool IXmlSchemaElement.IsAbstract() => e.IsAbstract;
        XmlSchemaForm IXmlSchemaElement.Form() => e.Form;
        IXmlQualifiedName IXmlSchemaElement.SchemaTypeName() => new TXmlQualifiedName(e.SchemaTypeName);
        string IXmlSchemaElement.FixedValue() => e.FixedValue;
        XmlSchemaDerivationMethod IXmlSchemaElement.Final() => e.Final;
        IXmlSchemaType IXmlSchemaElement.ElementSchemaType() => TXmlSchemaType.GetXmlSchemaType(e.ElementSchemaType);
        string IXmlSchemaElement.DefaultValue() => e.DefaultValue;
        XmlSchemaDerivationMethod IXmlSchemaElement.BlockResolved() => e.BlockResolved;
        XmlSchemaDerivationMethod IXmlSchemaElement.Block() => e.Block;
        XmlSchemaDerivationMethod IXmlSchemaElement.FinalResolved() => e.FinalResolved;
        IXmlQualifiedName IXmlSchemaElement.SubstitutionGroup() => new TXmlQualifiedName(e.SubstitutionGroup);
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaElement(XmlSchemaObject e, out IXmlSchemaElement outD)
        {
            outD = null;
            if (e is XmlSchemaElement) outD = new TXmlSchemaElement(e as XmlSchemaElement);
          //  else if (e is XmlSchemaChoice) outD = new TXmlSchemaChoice(e as XmlSchemaChoice);
            //TODO:XmlSchemaAny
            else MessageBox.Show("EXCEPTION is NOT XmlSchemaElement " + e.ToString());
        }
            
    }
/// <summary>
/// Attribute
/// </summary>
    [ComImport, Guid("CD35A56F-09F2-4071-B739-2C115DB7D58F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAttribute
    {
        //
        // Сводка:
        //     Gets an System.Xml.Schema.XmlSchemaSimpleType object representing the type of
        //     the attribute based on the System.Xml.Schema.XmlSchemaAttribute.SchemaType or
        //     System.Xml.Schema.XmlSchemaAttribute.SchemaTypeName of the attribute.
        //
        // Возврат:
        //     An System.Xml.Schema.XmlSchemaSimpleType object.

        [return: MarshalAs(UnmanagedType.Interface)]
        IXmlSchemaSimpleType AttributeSchemaType();
        //
        // Сводка:
        //     Gets or sets the default value for the attribute.
        //
        // Возврат:
        //     The default value for the attribute. The default is a null reference. Optional.
        //[DefaultValue(null)]
        //[XmlAttribute("default")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DefaultValue();
        //
        // Сводка:
        //     Gets or sets the fixed value for the attribute.
        //
        // Возврат:
        //     The fixed value for the attribute. The default is null. Optional.
        //[DefaultValue(null)]
        //[XmlAttribute("fixed")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string FixedValue();
        //
        // Сводка:
        //     Gets or sets the form for the attribute.
        //
        // Возврат:
        //     One of the System.Xml.Schema.XmlSchemaForm values. The default is the value of
        //     the System.Xml.Schema.XmlSchema.AttributeFormDefault of the schema element containing
        //     the attribute. Optional.
        //[DefaultValue(XmlSchemaForm.None)]
        //[XmlAttribute("form")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaForm Form();
        //
        // Сводка:
        //     Gets or sets the name of the attribute.
        //
        // Возврат:
        //     The name of the attribute.
        //[XmlAttribute("name")]

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        //
        // Сводка:
        //     Gets the qualified name for the attribute.
        //
        // Возврат:
        //     The post-compilation value of the QualifiedName property.

        IXmlQualifiedName QualifiedName();
        //
        // Сводка:
        //     Gets or sets the name of an attribute declared in this schema (or another schema
        //     indicated by the specified namespace).
        //
        // Возврат:
        //     The name of the attribute declared.
        //[XmlAttribute("ref")]

        IXmlQualifiedName RefName();
        //
        // Сводка:
        //     Gets or sets the attribute type to a simple type.
        //
        // Возврат:
        //     The simple type defined in this schema.
        //[XmlElement("simpleType")]

        IXmlSchemaSimpleType SchemaType();
        //
        // Сводка:
        //     Gets or sets the name of the simple type defined in this schema (or another schema
        //     indicated by the specified namespace).
        //
        // Возврат:
        //     The name of the simple type.
        //[XmlAttribute("type")]

        IXmlQualifiedName SchemaTypeName();
        //
        // Сводка:
        //     Gets or sets information about how the attribute is used.
        //
        // Возврат:
        //     One of the following values: None, Prohibited, Optional, or Required. The default
        //     is Optional. Optional.
        //[DefaultValue(XmlSchemaUse.None)]
        // [XmlAttribute("use")]

        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaUse Use();
    }
    public class TXmlSchemaAttribute : TXmlSchemaAnnotated, IXmlSchemaAttribute
    {
        private readonly XmlSchemaAttribute a;
        public TXmlSchemaAttribute(XmlSchemaAttribute a) : base(a) => this.a = a;
        IXmlSchemaSimpleType IXmlSchemaAttribute.AttributeSchemaType() => new TXmlSchemaSimpleType(a.AttributeSchemaType);
        string IXmlSchemaAttribute.DefaultValue() => a.DefaultValue;
        string IXmlSchemaAttribute.FixedValue() => a.FixedValue;
        XmlSchemaForm IXmlSchemaAttribute.Form() => a.Form;
        string IXmlSchemaAttribute.Name() => a.Name;
        IXmlQualifiedName IXmlSchemaAttribute.QualifiedName() => new TXmlQualifiedName(a.QualifiedName);
        IXmlQualifiedName IXmlSchemaAttribute.RefName() => new TXmlQualifiedName(a.RefName);
        IXmlSchemaSimpleType IXmlSchemaAttribute.SchemaType() => new TXmlSchemaSimpleType(a.SchemaType);
        IXmlQualifiedName IXmlSchemaAttribute.SchemaTypeName() => new TXmlQualifiedName(a.SchemaTypeName);
        XmlSchemaUse IXmlSchemaAttribute.Use() => a.Use;
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaAttribute(XmlSchemaObject a, out IXmlSchemaAttribute OutD) => OutD = new TXmlSchemaAttribute(a as XmlSchemaAttribute);
    }
/// <summary>
/// Schema Set
/// </summary>
    /*  ======= интерфейс реализован в Delphi ============
        ICSharpXMLValidatorCallBack = interface
            ['{AE7AF832-8353-48DC-966D-E6FE7737F171}']
            procedure ValidationCallback(SeverityType: integer; ErrorMessage: PChar); safecall;
        end; 
    ============================================= */
    [ComImport, Guid("AE7AF832-8353-48DC-966D-E6FE7737F171"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXMLValidatorCallBack
    {

        void ValidationCallback([MarshalAs(UnmanagedType.I4)] XmlSeverityType SeverityType,
                            [MarshalAs(UnmanagedType.LPWStr)] string ErrorMessage);
    }

    [ComImport, Guid("79CED196-FC1C-4032-8CB8-A6CD1E6C305C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlNamespaceManager
    {
        ///
        /// Сводка:
        ///     Возвращает универсальный код ресурса (URI) для пространства имен по умолчанию.
        ///
        /// Возврат:
        ///     Возвращает URI для пространства имен по умолчанию или String.Empty, если пространство
        ///     имен по умолчанию отсутствует.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string DefaultNamespace();
        ///
        /// Сводка:
        ///     Возвращает значение, указывающее, определено ли пространство имен для указанного
        ///     префикса в текущей области видимости, занесенной в стек.
        ///
        /// Параметры:
        ///   prefix:
        ///     Префикс пространства имен, которое требуется найти.
        ///
        /// Возврат:
        ///     trueЕсли имеется определенное пространство имен; в противном случае false.
        [return: MarshalAs(UnmanagedType.U1)]
        bool HasNamespace([MarshalAs(UnmanagedType.LPWStr)] string prefix);
        ///
        /// Сводка:
        ///     Возвращает URI пространства имен для указанного префикса.
        ///
        /// Параметры:
        ///   prefix:
        ///     Префикс, для которого требуется разрешить URI пространства имен. Чтобы сопоставить
        ///     пространство имен по умолчанию, необходимо передать String.Empty.
        ///
        /// Возврат:
        ///     Возвращает URI пространства имен для prefix или null Если соответствующее пространство
        ///     имен отсутствует. Возвращаемая строка является атомизированной. Дополнительные
        ///     сведения о разъединенных строках см. в разделе System.Xml.XmlNameTable класса.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LookupNamespace([MarshalAs(UnmanagedType.LPWStr)] string prefix);
        ///
        /// Сводка:
        ///     Находит префикс, объявленный для заданного URI пространства имен.
        ///
        /// Параметры:
        ///   uri:
        ///     Пространство имен, чтобы разрешить для получения префикса.
        ///
        /// Возврат:
        ///     Соответствующий префикс. Если нет сопоставленного префикса, данный метод возвращает
        ///     String.Empty. Если указано значение null, затем null возвращается.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LookupPrefix([MarshalAs(UnmanagedType.LPWStr)] string uri);
    }
    public class TXmlNamespaceManager : IXmlNamespaceManager
    {
        public readonly XmlNamespaceManager m;
        public TXmlNamespaceManager(XmlNameTable nameTable)=> m = new XmlNamespaceManager(nameTable);
        string IXmlNamespaceManager.DefaultNamespace() => m.DefaultNamespace;
        bool IXmlNamespaceManager.HasNamespace(string prefix) => m.HasNamespace(prefix);
        string IXmlNamespaceManager.LookupNamespace(string prefix) => m.LookupNamespace(prefix);
        string IXmlNamespaceManager.LookupPrefix(string uri) => m.LookupPrefix(uri);
    }
    [ComImport, Guid("CEAD7A91-2DAC-44E1-8425-F32E1A23DCE3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaSet
    {
        IXmlNamespaceManager Namespace();
        //
        // Сводка:
        //     Получает все глобальные атрибуты в определении схемы XML схем языка XSD в System.Xml.Schema.XmlSchemaSet.
        //
        // Возврат:
        //     Коллекция глобальных атрибутов.
        IXmlSchemaObjectTable GlobalAttributes();
        //
        // Сводка:
        //     Получает все глобальные элементы в определении схемы XML схем языка XSD в System.Xml.Schema.XmlSchemaSet.
        //
        // Возврат:
        //     Коллекция глобальных элементов.         
        IXmlSchemaObjectTable GlobalElements();
        //
        // Сводка:
        //     Получает все глобальные простые и сложные типы в определении схемы XML схем языка
        //     XSD в System.Xml.Schema.XmlSchemaSet.
        //
        // Возврат:
        //     Коллекция глобальных простых и сложных типов.         
        IXmlSchemaObjectTable GlobalTypes();
        void Add([MarshalAs(UnmanagedType.LPWStr)] string nameSpase, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        void Compile();
        //
        // Сводка:
        //     Возвращает значение, указывающее, является ли схемами языка определения схемы
        //     XML в System.Xml.Schema.XmlSchemaSet были скомпилированы.
        //
        // Возврат:
        //     true Если схемы в System.Xml.Schema.XmlSchemaSet были скомпилированы с момента
        //     последнего схемы был добавлен или удален из System.Xml.Schema.XmlSchemaSet; в
        //     противном случае — false.
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsCompiled();
        //
        // Сводка:
        //     Указывает обработчик событий, получающий сведения об ошибках проверки схем языка
        //     определения схем XML (XSD).
        void AddValidationEventHandler(IXMLValidatorCallBack v);
        //
        // Сводка:
        //     Повторная обработка схему языка XSD определения схемы XML, который уже существует
        //     в System.Xml.Schema.XmlSchemaSet.
        //
        // Параметры:
        //   schema:
        //     Схема, которую необходимо обработать повторно.
        //
        // Возврат:
        //     System.Xml.Schema.XmlSchema Объекта, если схема является допустимой схемой. Если
        //     схема не является допустимым и System.Xml.Schema.ValidationEventHandler указан,
        //     null возвращается и возникает соответствующее событие проверки. В противном случае
        //     — System.Xml.Schema.XmlSchemaException возникает исключение.
        //
        // Исключения:
        //   T:System.Xml.Schema.XmlSchemaException:
        //     Недопустимая схема.
        //
        //   T:System.ArgumentNullException:
        //     System.Xml.Schema.XmlSchema Объект, передаваемый как параметр — null.
        //
        //   T:System.ArgumentException:
        //     System.Xml.Schema.XmlSchema Объект, передаваемый как параметр еще не существует
        //     в System.Xml.Schema.XmlSchemaSet.
        IXmlSchema Reprocess(IXmlSchema schema);
        //
        // Сводка:
        //     Возвращает коллекцию всех определения схемы XML схем языка XSD в System.Xml.Schema.XmlSchemaSet.
        //
        // Возврат:
        //     System.Collections.ICollection Объект, содержащий все схемы, которые были добавлены
        //     в System.Xml.Schema.XmlSchemaSet. Если схемы не были добавлены в System.Xml.Schema.XmlSchemaSet,
        //     пустой System.Collections.ICollection возвращается объект.
        IXMLEnumerable Schemas();
        //
        // Сводка:
        //     Возвращает коллекцию всех определения схемы XML схем языка XSD в System.Xml.Schema.XmlSchemaSet
        //     относящихся к данному пространству имен.
        //
        // Параметры:
        //   targetNamespace:
        //     Схема targetNamespace свойство.
        //
        // Возврат:
        //     System.Collections.ICollection Объект, содержащий все схемы, которые были добавлены
        //     в System.Xml.Schema.XmlSchemaSet относящихся к данному пространству имен. Если
        //     схемы не были добавлены в System.Xml.Schema.XmlSchemaSet, пустой System.Collections.ICollection
        //     возвращается объект.
        IXMLEnumerable Schemas([MarshalAs(UnmanagedType.LPWStr)] string targetNamespace);
        void Validate([MarshalAs(UnmanagedType.LPWStr)] string FileName);
    }
    public class TXmlSchemaSet : IXmlSchemaSet
    {
        private TXmlNamespaceManager m;
        private IXMLValidatorCallBack cbk;
        private readonly XmlSchemaSet s;
        public TXmlSchemaSet()
        {
            s = new XmlSchemaSet();
            s.XmlResolver = new XmlUrlResolver(); // ОБЯЗАТЕЛЬНО!!!
            s.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            m = new TXmlNamespaceManager(s.NameTable);
        }
        void ValidationCallback(object sender, ValidationEventArgs args)=> cbk?.ValidationCallback(args.Severity, args.Message);
        IXmlSchemaObjectTable IXmlSchemaSet.GlobalAttributes() => new TXmlSchemaObjectTable(s.GlobalAttributes);
        public IXmlNamespaceManager Namespace() => m;
        IXmlSchemaObjectTable IXmlSchemaSet.GlobalElements() => new TXmlSchemaObjectTable(s.GlobalElements);
        IXmlSchemaObjectTable IXmlSchemaSet.GlobalTypes() => new TXmlSchemaObjectTable(s.GlobalTypes);
        void IXmlSchemaSet.Add(string nameSpase, string FileName) => s.Add(nameSpase, FileName);
        void IXmlSchemaSet.Compile()
        {
            s.Compile();
            foreach(XmlSchema s in s.Schemas())
            {
                foreach(XmlQualifiedName n in s.Namespaces.ToArray())
                {
                    m.m.AddNamespace(n.Name, n.Namespace);
                }
            }
        }
            
        bool IXmlSchemaSet.IsCompiled() => s.IsCompiled;
        void IXmlSchemaSet.AddValidationEventHandler(IXMLValidatorCallBack v) => cbk = v;
        IXmlSchema IXmlSchemaSet.Reprocess(IXmlSchema schema) => new TXmlSchema(s.Reprocess((schema as TXmlSchema).s));
        IXMLEnumerable IXmlSchemaSet.Schemas() => new TXMLEnumerable(s.Schemas());
        IXMLEnumerable IXmlSchemaSet.Schemas(string targetNamespace) => new TXMLEnumerable(s.Schemas(targetNamespace));
        void IXmlSchemaSet.Validate(string FileName)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = s
            };
            settings.ValidationEventHandler += ValidationCallback;

            try
            {
                XmlReader reader = XmlReader.Create(FileName, settings);
                // Parse the file.
                while (reader.Read()) { };
            }
            catch (Exception e)
            {
                cbk.ValidationCallback(XmlSeverityType.Error, FileName + " | " + e.Message);
            }
        }
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaSet(out IXmlSchemaSet OutD) => OutD = new TXmlSchemaSet();
    }
/// <summary>
/// Misc
/// </summary>
    [ComImport, Guid("D969D90F-16C4-4463-848B-1E8B255286B2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaGroup
    {
        ///
        /// Сводка:
        ///     Возвращает или задает имя группы схем.
        ///
        /// Возврат:
        ///     Имя группы схем.
        /// [XmlAttribute("name")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        ///
        /// Сводка:
        ///     Возвращает или задает один из System.Xml.Schema.XmlSchemaChoice, System.Xml.Schema.XmlSchemaAll,
        ///     или System.Xml.Schema.XmlSchemaSequence классы.
        ///
        /// Возврат:
        ///     Один из System.Xml.Schema.XmlSchemaChoice, System.Xml.Schema.XmlSchemaAll, или
        ///     System.Xml.Schema.XmlSchemaSequence классы.
        ///[XmlElement("sequence", typeof(XmlSchemaSequence))]
        ///[XmlElement("choice", typeof(XmlSchemaChoice))]
        ///[XmlElement("all", typeof(XmlSchemaAll))]
        IXmlSchemaParticle Particle();
        ///
        /// Сводка:
        ///     Возвращает полное имя группы схем.
        ///
        /// Возврат:
        ///     System.Xml.XmlQualifiedName Объект, представляющий полное имя группы схем.
        ///[XmlIgnore]
        IXmlQualifiedName QualifiedName();
    }
    public class TXmlSchemaGroup : TXmlSchemaAnnotated, IXmlSchemaGroup
    {
        public TXmlSchemaGroup(XmlSchemaGroup g) : base(g) { }
        string IXmlSchemaGroup.Name() => (x as XmlSchemaGroup).Name;
        IXmlSchemaParticle IXmlSchemaGroup.Particle() => TXmlSchemaParticle.GetPaticle((x as XmlSchemaGroup).Particle);
        IXmlQualifiedName IXmlSchemaGroup.QualifiedName() => new TXmlQualifiedName((x as XmlSchemaGroup).QualifiedName);
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaGroup(XmlSchemaObject a, out IXmlSchemaGroup OutD) => OutD = new TXmlSchemaGroup(a as XmlSchemaGroup);
    }

    [ComImport, Guid("1E47DBD5-7750-4B49-A8FE-980BD5683D9F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAttributeGroup
    {
        ///
        /// Сводка:
        ///     Возвращает или задает имя группы атрибутов.
        ///
        /// Возврат:
        ///     Имя группы атрибутов.
        ///[XmlAttribute("name")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        ///
        /// Сводка:
        ///     Возвращает коллекцию атрибутов для группы атрибутов. Содержит XmlSchemaAttribute
        ///     и XmlSchemaAttributeGroupRef элементы.
        ///
        /// Возврат:
        ///     Коллекция атрибутов для группы атрибутов.
        ///[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
        ///[XmlElement("attribute", typeof(XmlSchemaAttribute))]
        IXmlSchemaObjectCollection Attributes();
        ///
        /// Сводка:
        ///     Возвращает или задает System.Xml.Schema.XmlSchemaAnyAttribute компонент группы
        ///     атрибутов.
        ///
        /// Возврат:
        ///     World Wide Web Consortium (W3C) anyAttribute элемента.
        ///[XmlElement("anyAttribute")]
        /// public XmlSchemaAnyAttribute AnyAttribute { get; set; }
        ///
        /// Сводка:
        ///     Возвращает полное имя группы атрибутов.
        ///
        /// Возврат:
        ///     Полное имя группы атрибутов.
        ///[XmlIgnore]
        IXmlQualifiedName QualifiedName();
        ///
        /// Сводка:
        ///     Возвращает переопределенное свойство группы атрибутов из схемы XML.
        ///
        /// Возврат:
        ///     Переопределенное свойство группы атрибутов.
        ///[XmlIgnore]
        IXmlSchemaAttributeGroup RedefinedAttributeGroup();
    }
    public class TXmlSchemaAttributeGroup : TXmlSchemaAnnotated, IXmlSchemaAttributeGroup
    {
        public TXmlSchemaAttributeGroup(XmlSchemaAttributeGroup ag) : base(ag) { }
        IXmlSchemaObjectCollection IXmlSchemaAttributeGroup.Attributes() => new TXmlSchemaObjectCollection((x as XmlSchemaAttributeGroup).Attributes);
        string IXmlSchemaAttributeGroup.Name() => (x as XmlSchemaAttributeGroup).Name;
        IXmlQualifiedName IXmlSchemaAttributeGroup.QualifiedName() => new TXmlQualifiedName((x as XmlSchemaAttributeGroup).QualifiedName);
        IXmlSchemaAttributeGroup IXmlSchemaAttributeGroup.RedefinedAttributeGroup() => new TXmlSchemaAttributeGroup((x as XmlSchemaAttributeGroup).RedefinedAttributeGroup);
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaAttributeGroup(XmlSchemaObject a, out IXmlSchemaAttributeGroup OutD) => OutD = new TXmlSchemaAttributeGroup(a as XmlSchemaAttributeGroup);
    }

    [ComImport, Guid("04599C1A-F064-4661-90C4-326264600E79"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaNotation
    {
        ///
        /// Сводка:
        ///     Возвращает или задает имя нотации.
        ///
        /// Возврат:
        ///     Имя нотации.
        ///[XmlAttribute("name")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        ///
        /// Сводка:
        ///     Возвращает или задает public идентификатор.
        ///
        /// Возврат:
        ///     public Идентификатор. Значение должно быть действительным идентификатором URI.
        ///[XmlAttribute("public")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Public();
        ///
        /// Сводка:
        ///     Возвращает или задает system идентификатор.
        ///
        /// Возврат:
        ///     system Идентификатор. Значение должно быть действительным универсальным кодом
        ///     ресурса (URI).
        /// [XmlAttribute("system")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string System();
    }
    public class TXmlSchemaNotation : TXmlSchemaAnnotated, IXmlSchemaNotation
    {
        public TXmlSchemaNotation(XmlSchemaNotation n) : base(n) { }
        string IXmlSchemaNotation.Name() => (x as XmlSchemaNotation).Name;
        string IXmlSchemaNotation.Public() => (x as XmlSchemaNotation).Public;
        string IXmlSchemaNotation.System() => (x as XmlSchemaNotation).System;
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaNotation(XmlSchemaObject a, out IXmlSchemaNotation OutD) => OutD = new TXmlSchemaNotation(a as XmlSchemaNotation);
    }

    [ComImport, Guid("C2BD2AFC-2ACB-458E-8D30-3417E15C2998"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAnnotation
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAnnotation();
    }
    public class TXmlSchemaAnnotation : TXmlSchemaObject, IXmlSchemaAnnotation
    {
        public TXmlSchemaAnnotation(XmlSchemaAnnotation a) : base(a) { } 
        string IXmlSchemaAnnotation.GetAnnotation()
        {
            return TXmlSchemaAnnotated.GetDoc(x as XmlSchemaAnnotation);
        }
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaAnnotation(XmlSchemaObject a, out IXmlSchemaAnnotation OutD) => OutD = new TXmlSchemaAnnotation(a as XmlSchemaAnnotation);
    }

    [ComImport, Guid("FF2F3F61-DC24-4069-A6CF-87C709956EB5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaExternal
    {
        ///
        /// Сводка:
        ///     Возвращает или задает расположение универсальный код ресурса (URI) для схемы,
        ///     сообщающий процессору схемы, где физическое расположение.
        ///
        /// Возврат:
        ///     Расположение URI для схемы. Необязательно для импортированной схемы.
        ///[XmlAttribute("schemaLocation", DataType = "anyURI")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string SchemaLocation();
        ///
        /// Сводка:
        ///     Возвращает или задает XmlSchema для указанной схемы.
        ///
        /// Возврат:
        ///     XmlSchema Для указанной схемы.
        /// [XmlIgnore]
        IXmlSchema Schema();
        ///
        /// Сводка:
        ///     Возвращает или задает идентификатор строки.
        ///
        /// Возврат:
        ///     Идентификатор строки. Значение по умолчанию — String.Empty. Необязательно.
        /// [XmlAttribute("id", DataType = "ID")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Id();
    }
    public abstract class TXmlSchemaExternal: TXmlSchemaObject, IXmlSchemaObject, IXmlSchemaExternal
    {
        protected TXmlSchemaExternal(XmlSchemaExternal ex) : base(ex) { }
        string IXmlSchemaExternal.SchemaLocation() => (x as XmlSchemaExternal).SchemaLocation;
        IXmlSchema IXmlSchemaExternal.Schema() => new TXmlSchema((x as XmlSchemaExternal).Schema);
        string IXmlSchemaExternal.Id() => (x as XmlSchemaExternal).Id;
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaExternal(XmlSchemaObject a, out IXmlSchemaExternal OutD)
        {
            if (a is XmlSchemaImport) OutD = new TXmlSchemaImport(a as XmlSchemaImport);
            else if (a is XmlSchemaInclude) OutD = new TXmlSchemaInclude(a as XmlSchemaInclude);
            else OutD = null;
        }
    }
    [ComImport, Guid("12AF0716-D648-42EC-8921-4A88FA2B0300"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaImport
    {
        ///
        /// Сводка:
        ///     Возвращает или задает целевое пространство имен для импортированной схемы в виде
        ///     ссылки URI.
        ///
        /// Возврат:
        ///     Целевое пространство имен для импортированной схемы в виде ссылки URI. Необязательно.
        /// [XmlAttribute("namespace", DataType = "anyURI")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Namespace();
        ///
        /// Сводка:
        ///     Возвращает или задает свойство annotation.
        ///
        /// Возврат:
        ///     Примечание.
        /// [XmlElement("annotation", typeof(XmlSchemaAnnotation))]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAnnotation();

    }
    public class TXmlSchemaImport : TXmlSchemaExternal, IXmlSchemaExternal, IXmlSchemaImport, IXmlSchemaObject
    {
        public TXmlSchemaImport(XmlSchemaImport i) : base(i) { }
        string IXmlSchemaImport.Namespace() => (x as XmlSchemaImport).Namespace;
        string IXmlSchemaImport.GetAnnotation()
        {
            return TXmlSchemaAnnotated.GetDoc((x as XmlSchemaImport).Annotation);
        }
    }
    [ComImport, Guid("9468FBB5-BD0C-4F8B-BFB3-D865E2489C2A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaInclude
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAnnotation();
    }
    public class TXmlSchemaInclude : TXmlSchemaExternal, IXmlSchemaExternal, IXmlSchemaInclude, IXmlSchemaObject
    {
        public TXmlSchemaInclude(XmlSchemaInclude i) : base(i) { }
        string IXmlSchemaInclude.GetAnnotation()
        {
            return TXmlSchemaAnnotated.GetDoc((x as XmlSchemaInclude).Annotation);
        }
    }

    [ComImport, Guid("BE994973-F9BA-4B4B-AC4B-B704313EA5D5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaAny
    {
        ///
        /// Сводка:
        ///     Возвращает или задает пространство имен, содержащее элементы, которые можно использовать.
        ///
        /// Возврат:
        ///     Пространства имен для элементов, доступных для использования. Значение по умолчанию
        ///     — ##any. Необязательно.
        /// [XmlAttribute("namespace")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Namespace();
        ///
        /// Сводка:
        ///     Возвращает или задает сведения о том, как приложение или процессор XML должен
        ///     осуществлять проверку документов XML на предмет наличия элементов, определенных
        ///     any элемента.
        ///
        /// Возврат:
        ///     Одно из значений System.Xml.Schema.XmlSchemaContentProcessing. Если не processContents
        ///     атрибут указан, значение по умолчанию — Strict.
        /// [DefaultValue(XmlSchemaContentProcessing.None)]
        /// [XmlAttribute("processContents")]
        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaContentProcessing ProcessContents();
    }
    public class TXmlSchemaAny: TXmlSchemaParticle, IXmlSchemaAny, IXmlSchemaParticle, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaAny(XmlSchemaAny a) : base(a) { }
        string IXmlSchemaAny.Namespace() => (x as XmlSchemaAny).Namespace;
        XmlSchemaContentProcessing IXmlSchemaAny.ProcessContents() => (x as XmlSchemaAny).ProcessContents;
    }
}


