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
        ///
        /// Сводка:
        ///     Возвращает или задает число как строковое значение. Минимальное количество раз,
        ///     которое может встречаться фрагмент.
        ///
        /// Возврат:
        ///     Число как строковое значение. String.Empty Указывает, что MinOccurs равно значению
        ///     по умолчанию. Значение по умолчанию: пустая ссылка.
        /// [XmlAttribute("minOccurs")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string MinOccursString();
        ///
        /// Сводка:
        ///     Возвращает или задает число как строковое значение. Максимальное количество раз,
        ///     которое может встречаться фрагмент.
        ///
        /// Возврат:
        ///     Число как строковое значение. String.Empty Указывает, что MaxOccurs равно значению
        ///     по умолчанию. Значение по умолчанию: пустая ссылка.
        /// [XmlAttribute("maxOccurs")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string MaxOccursString();
    }
    public abstract class TXmlSchemaParticle : TXmlSchemaAnnotated, IXmlSchemaParticle, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaParticle(XmlSchemaParticle p) : base(p) { }
        decimal IXmlSchemaParticle.MinOccurs() => (x as XmlSchemaParticle).MinOccurs;
        decimal IXmlSchemaParticle.MaxOccurs() => (x as XmlSchemaParticle).MaxOccurs;
        string IXmlSchemaParticle.MinOccursString() => (x as XmlSchemaParticle).MinOccursString;
        string IXmlSchemaParticle.MaxOccursString() => (x as XmlSchemaParticle).MaxOccursString;
        public static IXmlSchemaParticle GetPaticle(XmlSchemaParticle p)
        {
            if (p is XmlSchemaAll) return new TXmlSchemaAll(p as XmlSchemaAll);
            else if (p is XmlSchemaChoice) return new TXmlSchemaChoice(p as XmlSchemaChoice);
            else if (p is XmlSchemaSequence) return new TXmlSchemaSequence(p as XmlSchemaSequence);
            else if (p is XmlSchemaGroupRef) return new TXmlSchemaGroupRef(p as XmlSchemaGroupRef);
            else if (p is XmlSchemaAny) return new TXmlSchemaAny(p as XmlSchemaAny);
//            else if (p is XmlSchemaElement) return new TXmlSchemaElement(p as XmlSchemaElement);
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
    public abstract class TXmlSchemaExternal : TXmlSchemaObject, IXmlSchemaObject, IXmlSchemaExternal
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
    public class TXmlSchemaAny : TXmlSchemaParticle, IXmlSchemaAny, IXmlSchemaParticle, IXmlSchemaAnnotated, IXmlSchemaObject
    {
        public TXmlSchemaAny(XmlSchemaAny a) : base(a) { }
        string IXmlSchemaAny.Namespace() => (x as XmlSchemaAny).Namespace;
        XmlSchemaContentProcessing IXmlSchemaAny.ProcessContents() => (x as XmlSchemaAny).ProcessContents;
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

    [ComImport, Guid("0B02EBF5-71EA-44BA-92A3-5373E05B06D5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IComXmlSchemaInfo
    {
        //
        // Сводка:
        //     Возвращает или задает System.Xml.Schema.XmlSchemaValidity проверяется значение
        //     этого XML-узла.
        //
        // Возврат:
        //     Значение System.Xml.Schema.XmlSchemaValidity.
        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaValidity Validity();
        //
        // Сводка:
        //     Возвращает или задает значение, указывающее, если проверенный узел XML был установлен
        //     в результате значения по умолчанию, применяемого в ходе проверки схемы языка
        //     определения схем XML (XSD).
        //
        // Возврат:
        //     Значение bool.
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsDefault();
        //
        // Сводка:
        //     Возвращает или задает значение, указывающее, если проверенный узел XML значение
        //     nil.
        //
        // Возврат:
        //     Значение bool.
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsNil();
        //
        // Сводка:
        //     Возвращает или задает динамический тип схемы для этого проверенного XML-узла.
        //
        // Возврат:
        //     Объект System.Xml.Schema.XmlSchemaSimpleType.
        IXmlSchemaSimpleType MemberType();
        //
        // Сводка:
        //     Возвращает или задает статический тип схемы языка определения схем XML (XSD)
        //     проверенный узел XML.
        //
        // Возврат:
        //     Объект System.Xml.Schema.XmlSchemaType.
        IXmlSchemaType SchemaType();
        //
        // Сводка:
        //     Возвращает или задает скомпилированного System.Xml.Schema.XmlSchemaElement объект,
        //     соответствующий этому проверенный узел XML.
        //
        // Возврат:
        //     Объект System.Xml.Schema.XmlSchemaElement.
        IXmlSchemaElement SchemaElement();
        //
        // Сводка:
        //     Возвращает или задает скомпилированного System.Xml.Schema.XmlSchemaAttribute
        //     объект, соответствующий этому проверенный узел XML.
        //
        // Возврат:
        //     Объект System.Xml.Schema.XmlSchemaAttribute.
        IXmlSchemaAttribute SchemaAttribute();
        //
        // Сводка:
        //     Возвращает или задает System.Xml.Schema.XmlSchemaContentType объекта, который
        //     соответствует типу содержимого это проверенный узел XML.
        //
        // Возврат:
        //     Объект System.Xml.Schema.XmlSchemaContentType.
        [return: MarshalAs(UnmanagedType.U4)]
        XmlSchemaContentType ContentType();
    }
    public class TXmlSchemaInfo : IComXmlSchemaInfo
    {
        public readonly XmlSchemaInfo x;
        public readonly IXmlSchemaInfo ix;
        public TXmlSchemaInfo()
        {
            x = new XmlSchemaInfo();
            ix = x;
        }
        public TXmlSchemaInfo(IXmlSchemaInfo ix) => this.ix = ix;
        XmlSchemaValidity IComXmlSchemaInfo.Validity() => ix.Validity;
        bool IComXmlSchemaInfo.IsDefault() => ix.IsDefault;
        bool IComXmlSchemaInfo.IsNil() => ix.IsNil;
        IXmlSchemaSimpleType IComXmlSchemaInfo.MemberType() => (ix.MemberType != null) ? new TXmlSchemaSimpleType(ix.MemberType) : null; 
        IXmlSchemaType IComXmlSchemaInfo.SchemaType() => (ix.SchemaType != null) ? TXmlSchemaType.GetXmlSchemaType(ix.SchemaType): null;
        IXmlSchemaElement IComXmlSchemaInfo.SchemaElement() => (ix.SchemaElement != null) ? new TXmlSchemaElement(ix.SchemaElement) : null;
        IXmlSchemaAttribute IComXmlSchemaInfo.SchemaAttribute() => (ix.SchemaAttribute != null) ? new TXmlSchemaAttribute(ix.SchemaAttribute) : null;
        XmlSchemaContentType IComXmlSchemaInfo.ContentType() => x.ContentType;
    }

    [ComImport, Guid("C565ABD6-CE6F-48D2-B796-505936CEAD9C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlSchemaValidator
    {
        ///
        /// Сводка:
        ///     System.Xml.Schema.ValidationEventHandler Получает предупреждений проверки схемы
        ///     и ошибки, возникшие при проверке схемы.
        void AddValidationEventHandler(IXMLValidatorCallBack v);
        ///
        /// Сводка:
        ///     Завершает проверку и проверяет ограничения идентификации для всего документа
        ///     XML.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Ошибка ограничения идентификации найден в XML-документе.
        void EndValidation();
        ///
        /// Сводка:
        ///     Возвращает ожидаемые атрибуты для контекста текущего элемента.
        ///
        /// Возврат:
        ///     Массив System.Xml.Schema.XmlSchemaAttribute объектов или пустой массив, если
        ///     ожидаемые атрибуты отсутствуют.
        void GetExpectedAttributes([MarshalAs(UnmanagedType.LPArray)] out XmlSchemaAttribute[] Attributes, out int Count);
        ///
        /// Сводка:
        ///     Возвращает ожидаемые примитивы в контексте текущего элемента.
        ///
        /// Возврат:
        ///     Массив System.Xml.Schema.XmlSchemaParticle объектов или пустой массив, если отсутствуют
        ///     указанные примитивы.
        void GetExpectedParticles([MarshalAs(UnmanagedType.LPArray)] out XmlSchemaParticle[] Particles, out int Count);
        ///
        /// Сводка:
        ///     Проверяет ограничения идентификации в атрибуты по умолчанию и заполняет System.Collections.ArrayList
        ///     задается с помощью System.Xml.Schema.XmlSchemaAttribute объектов для любых атрибутов
        ///     со значениями по умолчанию, которые не были проверены ранее с помощью Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute
        ///     метод в контексте элемента.
        ///
        /// Параметры:
        ///   defaultAttributes:
        ///     System.Collections.ArrayList Для заполнения System.Xml.Schema.XmlSchemaAttribute
        ///     объектов для всех атрибутов, которые не были обнаружены во время проверки в контексте
        ///     элемента.
        void GetUnspecifiedDefaultAttributes([MarshalAs(UnmanagedType.LPArray)] out object[] defaultAttributes, out int Count);
        ///
        /// Сводка:
        ///     Инициализирует состояние System.Xml.Schema.XmlSchemaValidator объекта.
        ///
        /// Исключения:
        ///   T:System.InvalidOperationException:
        ///     Вызов Overload:System.Xml.Schema.XmlSchemaValidator.Initialize метод допустим
        ///     сразу после создания System.Xml.Schema.XmlSchemaValidator объекта или после вызова
        ///     System.Xml.Schema.XmlSchemaValidator.EndValidation только.
        void Initialize();
        ///
        /// Сводка:
        ///     Инициализирует состояние System.Xml.Schema.XmlSchemaValidator с помощью System.Xml.Schema.XmlSchemaObject
        ///     для частичной проверки.
        ///
        /// Параметры:
        ///   partialValidationType:
        ///     System.Xml.Schema.XmlSchemaElement, System.Xml.Schema.XmlSchemaAttribute, Или
        ///     System.Xml.Schema.XmlSchemaType объект, используемый для инициализации контекста
        ///     проверки объекта System.Xml.Schema.XmlSchemaValidator объекта для частичной проверки.
        ///
        /// Исключения:
        ///   T:System.InvalidOperationException:
        ///     Вызов Overload:System.Xml.Schema.XmlSchemaValidator.Initialize метод допустим
        ///     сразу после создания System.Xml.Schema.XmlSchemaValidator объекта или после вызова
        ///     System.Xml.Schema.XmlSchemaValidator.EndValidation только.
        ///
        ///   T:System.ArgumentException:
        ///     System.Xml.Schema.XmlSchemaObject Параметр не System.Xml.Schema.XmlSchemaElement,
        ///     System.Xml.Schema.XmlSchemaAttribute, или System.Xml.Schema.XmlSchemaType объекта.
        ///
        ///   T:System.ArgumentNullException:
        ///     System.Xml.Schema.XmlSchemaObject Параметр не может быть null.
        void Initialize(IXmlSchemaObject partialValidationType);
        ///
        /// Сводка:
        ///     Пропускает проверку содержимого текущего элемента и подготавливает объект System.Xml.Schema.XmlSchemaValidator
        ///     для проверки содержимого в контексте родительского элемента.
        ///
        /// Параметры:
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Объект свойств которого задаются при пропуске
        ///     проверки содержимого текущего элемента. Этот параметр может иметь значение null.
        ///
        /// Исключения:
        ///   T:System.InvalidOperationException:
        ///     System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo)
        ///     Не был вызван метод в правильной последовательности. Например, вызов System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo)
        ///     после вызова метода System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo).
        void SkipToEndElement(out IComXmlSchemaInfo schemaInfo);
        ///
        /// Сводка:
        ///     Проверяет имя атрибута, URI пространства имен и значение в контексте текущего
        ///     элемента.
        ///
        /// Параметры:
        ///   localName:
        ///     Проверяемое локальное имя атрибута.
        ///
        ///   namespaceUri:
        ///     Проверяемый URI пространства имен атрибута.
        ///
        ///   attributeValue:
        ///     Проверяемое значение атрибута.
        ///
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Объект свойств которого задаются при успешной
        ///     проверке атрибута. Этот параметр может иметь значение null.
        ///
        /// Возврат:
        ///     Проверенное значение атрибута.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Атрибут не является допустимым в контексте текущего элемента.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute Не был вызван
        ///     метод в правильной последовательности. Например, вызов Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute
        ///     после вызова метода System.Xml.Schema.XmlSchemaValidator.ValidateEndOfAttributes(System.Xml.Schema.XmlSchemaInfo).
        ///
        ///   T:System.ArgumentNullException:
        ///     Одно или несколько из указанных параметров null.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ValidateAttribute([MarshalAs(UnmanagedType.LPWStr)] string localName,
                                 [MarshalAs(UnmanagedType.LPWStr)] string namespaceUri,
                                 [MarshalAs(UnmanagedType.LPWStr)] string attributeValue, out IComXmlSchemaInfo schemaInfo);
        ///
        /// Сводка:
        ///     Проверяет элемент в текущем контексте.
        ///
        /// Параметры:
        ///   localName:
        ///     Проверяемое локальное имя элемента.
        ///
        ///   namespaceUri:
        ///     Проверяемый URI пространства имен элемента.
        ///
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Объект свойств которого задаются при успешной
        ///     проверке имени элемента. Этот параметр может иметь значение null.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Недопустимое имя элемента в текущем контексте.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateElement Не был вызван метод
        ///     в правильной последовательности. Например Overload:System.Xml.Schema.XmlSchemaValidator.ValidateElement
        ///     метод вызывается после вызова метода Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute.
        void ValidateElement([MarshalAs(UnmanagedType.LPWStr)] string localName,
                             [MarshalAs(UnmanagedType.LPWStr)] string namespaceUri, out IComXmlSchemaInfo schemaInfo);
        ///
        /// Сводка:
        ///     Проверяет элемент в текущем контексте с xsi:Type, xsi:Nil, xsi:SchemaLocation,
        ///     и xsi:NoNamespaceSchemaLocation указанного значения атрибута.
        ///
        /// Параметры:
        ///   localName:
        ///     Проверяемое локальное имя элемента.
        ///
        ///   namespaceUri:
        ///     Проверяемый URI пространства имен элемента.
        ///
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Объект свойств которого задаются при успешной
        ///     проверке имени элемента. Этот параметр может иметь значение null.
        ///
        ///   xsiType:
        ///     xsi:Type Значение элемента атрибута. Этот параметр может иметь значение null.
        ///
        ///   xsiNil:
        ///     xsi:Nil Значение элемента атрибута. Этот параметр может иметь значение null.
        ///
        ///   xsiSchemaLocation:
        ///     xsi:SchemaLocation Значение элемента атрибута. Этот параметр может иметь значение
        ///     null.
        ///
        ///   xsiNoNamespaceSchemaLocation:
        ///     xsi:NoNamespaceSchemaLocation Значение элемента атрибута. Этот параметр может
        ///     иметь значение null.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Недопустимое имя элемента в текущем контексте.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateElement Не был вызван метод
        ///     в правильной последовательности. Например Overload:System.Xml.Schema.XmlSchemaValidator.ValidateElement
        ///     метод вызывается после вызова метода Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute.
        void ValidateElement([MarshalAs(UnmanagedType.LPWStr)] string localName,
                             [MarshalAs(UnmanagedType.LPWStr)] string namespaceUri, 
                             out IComXmlSchemaInfo schemaInfo,
                             [MarshalAs(UnmanagedType.LPWStr)] string xsiType,
                             [MarshalAs(UnmanagedType.LPWStr)] string xsiNil,
                             [MarshalAs(UnmanagedType.LPWStr)] string xsiSchemaLocation,
                             [MarshalAs(UnmanagedType.LPWStr)] string xsiNoNamespaceSchemaLocation);
        ///
        /// Сводка:
        ///     Проверяет, является ли текстовое содержимое указанного элемента допустимым для
        ///     его типа данных.
        ///
        /// Параметры:
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Объект свойств которого задаются при успешной
        ///     проверке текстового содержимого элемента. Этот параметр может иметь значение
        ///     null.
        ///
        ///   typedValue:
        ///     Типизированное текстовое содержимое элемента.
        ///
        /// Возврат:
        ///     Простое типизированное содержимое элемента после анализа.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Текстовое содержимое элемента является недопустимым.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateEndElement Не был вызван
        ///     метод в правильном порядке (например, если Overload:System.Xml.Schema.XmlSchemaValidator.ValidateEndElement
        ///     метод вызывается после вызова метода System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo)),
        ///     вызовы Overload:System.Xml.Schema.XmlSchemaValidator.ValidateText ранее были
        ///     внесены метод или элемент имеет сложное содержимое.
        ///
        ///   T:System.ArgumentNullException:
        ///     Параметр типизированного текстового содержимого не может быть null.
        [return: MarshalAs(UnmanagedType.Interface)]
        object ValidateEndElement(out IComXmlSchemaInfo schemaInfo, [MarshalAs(UnmanagedType.Interface)] object typedValue);
        ///
        /// Сводка:
        ///     Для элементов с простым содержимым проверяет, является ли текстовое содержимое
        ///     элемента допустимым для его типа данных. Для элементов со сложным содержимым
        ///     проверяет, является ли содержимое текущего элемента полным.
        ///
        /// Параметры:
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Свойств которого задаются при успешной проверке
        ///     элемента объекта. Этот параметр может иметь значение null.
        ///
        /// Возврат:
        ///     Проанализированное типизированное текстовое значение элемента, если содержимое
        ///     элемента является простым.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Недопустимое содержимое элемента.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateEndElement Не был вызван
        ///     метод в правильной последовательности. Например если Overload:System.Xml.Schema.XmlSchemaValidator.ValidateEndElement
        ///     метод вызывается после вызова метода System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo).
        [return: MarshalAs(UnmanagedType.Interface)] 
        object ValidateEndElement(out IComXmlSchemaInfo schemaInfo);
        ///
        /// Сводка:
        ///     Проверяет наличие всех необходимых атрибутов в контексте элемента и подготавливает
        ///     объект System.Xml.Schema.XmlSchemaValidator для проверки содержимого дочернего
        ///     элемента.
        ///
        /// Параметры:
        ///   schemaInfo:
        ///     System.Xml.Schema.XmlSchemaInfo Свойств которого задаются при успешной проверке,
        ///     что имеются все необходимые атрибуты контекста элемента на объект. Этот параметр
        ///     может иметь значение null.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Не удалось найти один или несколько необходимых атрибутов в контексте текущего
        ///     элемента.
        ///
        ///   T:System.InvalidOperationException:
        ///     System.Xml.Schema.XmlSchemaValidator.ValidateEndOfAttributes(System.Xml.Schema.XmlSchemaInfo)
        ///     Не был вызван метод в правильной последовательности. Например, вызов System.Xml.Schema.XmlSchemaValidator.ValidateEndOfAttributes(System.Xml.Schema.XmlSchemaInfo)
        ///     после вызова метода System.Xml.Schema.XmlSchemaValidator.SkipToEndElement(System.Xml.Schema.XmlSchemaInfo).
        ///
        ///   T:System.ArgumentNullException:
        ///     Одно или несколько из указанных параметров null.
        void ValidateEndOfAttributes(out IComXmlSchemaInfo schemaInfo);
        ///
        /// Сводка:
        ///     Проверяет, является ли текст string допустимым для контекста текущего элемента
        ///     и собирает текст для проверки, если текущий элемент имеет простое содержимое.
        ///
        /// Параметры:
        ///   elementValue:
        ///     Текстовый string нужно проверить в контексте текущего элемента.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Текст string указанного не разрешены в контексте текущего элемента.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateText Не был вызван метод
        ///     в правильной последовательности. Например Overload:System.Xml.Schema.XmlSchemaValidator.ValidateText
        ///     метод вызывается после вызова метода Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute.
        ///
        ///   T:System.ArgumentNullException:
        ///     Текст string параметр не может быть null.
        void ValidateText([MarshalAs(UnmanagedType.LPWStr)] string elementValue);
        ///
        /// Сводка:
        ///     Проверяет, является ли пустое пространство в string допустимым для контекста
        ///     текущего элемента и собирает пустое пространство для проверки, если текущий элемент
        ///     имеет простое содержимое.
        ///
        /// Параметры:
        ///   elementValue:
        ///     Пустое пространство string нужно проверить в контексте текущего элемента.
        ///
        /// Исключения:
        ///   T:System.Xml.Schema.XmlSchemaValidationException:
        ///     Пробел не допускается в контексте текущего элемента.
        ///
        ///   T:System.InvalidOperationException:
        ///     Overload:System.Xml.Schema.XmlSchemaValidator.ValidateWhitespace Не был вызван
        ///     метод в правильной последовательности. Например если Overload:System.Xml.Schema.XmlSchemaValidator.ValidateWhitespace
        ///     метод вызывается после вызова метода Overload:System.Xml.Schema.XmlSchemaValidator.ValidateAttribute.
        void ValidateWhitespace([MarshalAs(UnmanagedType.LPWStr)] string elementValue);
    }
    public class TXmlSchemaValidator : IXmlSchemaValidator
    {
        public readonly XmlSchemaValidator v;
        private IXMLValidatorCallBack cbk;
        private readonly TXmlSchemaInfo inf;
        public TXmlSchemaValidator(XmlNameTable nameTable, XmlSchemaSet schemas, IXmlNamespaceResolver namespaceResolver, XmlSchemaValidationFlags validationFlags)
        {
            v = new XmlSchemaValidator(nameTable, schemas, namespaceResolver, validationFlags);
            v.ValidationEventHandler += ValidationCallback;
            inf = new TXmlSchemaInfo();
        }
        void ValidationCallback(object sender, ValidationEventArgs args) => cbk?.ValidationCallback(args.Severity, args.Message);
        void IXmlSchemaValidator.AddValidationEventHandler(IXMLValidatorCallBack ev) => cbk = ev;
        void IXmlSchemaValidator.EndValidation() => v.EndValidation();
        void IXmlSchemaValidator.GetExpectedAttributes(out XmlSchemaAttribute[] Attributes, out int Count)
        {
            Attributes = v.GetExpectedAttributes();
            Count = Attributes.Length;
        }
        void IXmlSchemaValidator.GetExpectedParticles(out XmlSchemaParticle[] Particles, out int Count)
        {
            Particles = v.GetExpectedParticles();
            Count = Particles.Length;
        }
        void IXmlSchemaValidator.GetUnspecifiedDefaultAttributes(out object[] defaultAttributes, out int Count)
        {
            ArrayList a = new ArrayList();
            v.GetUnspecifiedDefaultAttributes(a);
            defaultAttributes = a.ToArray();
            Count = defaultAttributes.Length;
        }
        void IXmlSchemaValidator.Initialize() => v.Initialize();
        void IXmlSchemaValidator.Initialize(IXmlSchemaObject partialValidationType) => v.Initialize((XmlSchemaObject) partialValidationType.XmlObject());
        void IXmlSchemaValidator.SkipToEndElement(out IComXmlSchemaInfo schemaInfo)
        {
           v.SkipToEndElement(inf.x);
            schemaInfo = inf;
        }
        string IXmlSchemaValidator.ValidateAttribute(string localName, string namespaceUri, string attributeValue, out IComXmlSchemaInfo schemaInfo)
        {
            schemaInfo = inf;
            return v.ValidateAttribute(localName, namespaceUri, attributeValue, inf.x).ToString();
        }
        void IXmlSchemaValidator.ValidateElement(string localName, string namespaceUri, out IComXmlSchemaInfo schemaInfo)
        {
            schemaInfo = inf;
            v.ValidateElement(localName, namespaceUri, inf.x);
        }
        void IXmlSchemaValidator.ValidateElement(string localName, string namespaceUri, out IComXmlSchemaInfo schemaInfo, string xsiType, string xsiNil, string xsiSchemaLocation, string xsiNoNamespaceSchemaLocation)
        {
            schemaInfo = inf;
            v.ValidateElement(localName, namespaceUri, inf.x, xsiType, xsiNil, xsiSchemaLocation, xsiNoNamespaceSchemaLocation);
        }
        object IXmlSchemaValidator.ValidateEndElement(out IComXmlSchemaInfo schemaInfo, object typedValue)
        {
            schemaInfo = inf;
            return v.ValidateEndElement(inf.x, typedValue);
        }
        object IXmlSchemaValidator.ValidateEndElement(out IComXmlSchemaInfo schemaInfo)
        {
            schemaInfo = inf;
            return v.ValidateEndElement(inf.x);
        }
        void IXmlSchemaValidator.ValidateEndOfAttributes(out IComXmlSchemaInfo schemaInfo)
        {
            schemaInfo = inf;
            v.ValidateEndOfAttributes(inf.x);
        }
        void IXmlSchemaValidator.ValidateText(string elementValue) => v.ValidateText(elementValue);
        void IXmlSchemaValidator.ValidateWhitespace(string elementValue) => v.ValidateWhitespace(elementValue);
    }

    [ComImport, Guid("51B807B6-9806-4BAD-9179-1CCC82E6AEC0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXmlReader
    {
        //
        // Сводка:
        //     Возвращает значение, показывающее, имеются ли атрибуты у текущего узла.
        //
        // Возврат:
        //     Значение true, если текущий узел содержит атрибуты; в противном случае — значение
        //     false.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool HasAttributes();
        //
        // Сводка:
        //     При переопределении в производном классе, возвращает текущую xml:lang область.
        //
        // Возврат:
        //     Текущая область действия xml:lang.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string XmlLang();
        //
        // Сводка:
        //     При переопределении в производном классе, возвращает текущую xml:space область.
        //
        // Возврат:
        //     Одно из значений System.Xml.XmlSpace. Если область действия xml:space отсутствует,
        //     данное свойство принимает значение XmlSpace.None.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U4)]
        XmlSpace XmlSpace();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает значение, определяющее,
        //     является ли текущий узел атрибутом, созданным из значения по умолчанию, определенного
        //     в DTD или схеме.
        //
        // Возврат:
        //     Значение true, если текущий узел является атрибутом, значение которого было создано
        //     из значения по умолчанию, определенного в DTD или схеме; значение false, если
        //     значение атрибута было задано явно.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsDefault();
        //
        // Сводка:
        //     При переопределении в производном классе получает значение, указывающее, является
        //     ли текущий узел пустым элементом (например, <MyElement/>).
        //
        // Возврат:
        //     Значение true, если текущий узел является элементом (свойство System.Xml.XmlReader.NodeType
        //     имеет значение XmlNodeType.Element), который заканчивается на />; в противном
        //     случае — false.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsEmptyElement();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает базовый URI текущего узла.
        //
        // Возврат:
        //     Базовый URI текущего узла.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string BaseURI();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает текстовое значение текущего
        //     узла.
        //
        // Возврат:
        //     Возвращаемое значение зависит от значения свойства System.Xml.XmlReader.NodeType
        //     узла. В следующей таблице представлен список возвращаемых типов узлов со значениями.
        //     Все прочие типы узлов возвращают значение String.Empty. Тип узла Значение Attribute
        //     Значение атрибута. CDATA Содержимое раздела CDATA. Comment Содержимое комментария.
        //     DocumentType Внутреннее подмножество. ProcessingInstruction Все содержимое, за
        //     исключением цели. SignificantWhitespace Пробелы между разметкой в модели смешанного
        //     содержимого. Text Содержимое текстового узла. Whitespace Пробелы между разметкой.
        //     XmlDeclaration Содержимое декларации.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Value();
        //
        // Сводка:
        //     При переопределении в производном классе получает значение, указывающее, является
        //     ли текущий узел может иметь System.Xml.XmlReader.Value.
        //
        // Возврат:
        //     Значение true, если узел, на котором расположено средство чтения, может иметь
        //     значение Value; в противном случае — false. Если false, узел имеет значение String.Empty.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool HasValue();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает префикс пространства имен,
        //     связанный с текущим узлом.
        //
        // Возврат:
        //     Префикс пространства имен, связанный с текущим узлом.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Prefix();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает тип текущего узла.
        //
        // Возврат:
        //     Одно из значений перечисления, задающее тип текущего узла.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U4)]
        XmlNodeType NodeType();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает полное имя текущего узла.
        //
        // Возврат:
        //     Полное имя текущего узла. Например, Name имеет значение bk:book для элемента
        //     <bk:book>. Возвращаемое имя зависит от значения свойства System.Xml.XmlReader.NodeType
        //     узла. Значения возвращаются для представленных ниже типов узлов. Для других типов
        //     узлов возвращается пустая строка. Тип узла Имя Attribute Имя атрибута. DocumentType
        //     Имя типа документа. Element Имя тега. EntityReference Имя сущности, на которую
        //     существует ссылка. ProcessingInstruction Цель инструкции по обработке. XmlDeclaration
        //     Символьная строка xml.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string Name();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает глубину текущего узла в
        //     XML-документе.
        //
        // Возврат:
        //     Глубина текущего узла в XML-документе.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        int Depth();
        //
        // Сводка:
        //     Возвращает сведения схемы, которые были назначены текущему узлу в результате
        //     проверки схемы.
        //
        // Возврат:
        //     System.Xml.Schema.IXmlSchemaInfo Объект, содержащий сведения о схеме для текущего
        //     узла. Сведения о схеме можно задать на элементов, атрибутов или текстовых узлов
        //     с пустым System.Xml.XmlReader.ValueType (типизированные значения). Если текущий
        //     узел не является одним из приведенных выше типов узлов или XmlReader экземпляра
        //     не сообщает сведения о схеме, это свойство возвращает null. Если это свойство
        //     вызывается из System.Xml.XmlTextReader или System.Xml.XmlValidatingReader объекта,
        //     это свойство всегда возвращает null. Эти XmlReader реализации не раскрывают сведений
        //     схемы посредством SchemaInfo свойство. Если требуется получить информационный
        //     набор после проверки схемы (PSVI — post-schema-validation information set) для
        //     элемента, выполните позиционирование объекта чтения на конечный тег элемента
        //     вместо начального. PSVI доступны SchemaInfo объекта чтения. Проверяющий модуль
        //     чтения, созданного при помощи Overload:System.Xml.XmlReader.Create с System.Xml.XmlReaderSettings.ValidationType
        //     свойству System.Xml.ValidationType.Schema имеет полные сведения PSVI для элемента
        //     только в том случае, если средство чтения расположено на конечный тег элемента.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        IComXmlSchemaInfo SchemaInfo();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает количество атрибутов текущего
        //     узла.
        //
        // Возврат:
        //     Количество атрибутов текущего узла.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        int AttributeCount();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает локальное имя текущего
        //     узла.
        //
        // Возврат:
        //     Имя текущего узла с удаленным префиксом. Например, LocalName имеет значение book
        //     для элемента <bk:book>. Для безымянных типов узлов (например, Text, Comment и
        //     т. д.) данное свойство возвращает String.Empty.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LocalName();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает URI пространства имен (определенное
        //     в спецификации W3C Namespace) узла, на котором расположено средство чтения.
        //
        // Возврат:
        //     URI пространства имен текущего узла; в противном случае — пустая строка.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string NamespaceURI();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает состояние средства чтения.
        //
        // Возврат:
        //     Одно из значений перечисления, определяющее состояние средства чтения.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U4)]
        ReadState ReadState();
        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает значение, показывающее,
        //     позиционировано ли средство чтения в конец потока.
        //
        // Возврат:
        //     Значение true, если средство чтения установлено в конец потока; в противном случае
        //     — false.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool EOF();

        //
        // Сводка:
        //     Когда переопределено в производном классе, возвращает значение атрибута по указанному
        //     индексу.
        //
        // Параметры:
        //   i:
        //     Индекс атрибута. Индексация начинается с нуля. (Индекс первого атрибута равен
        //     нулю.)
        //
        // Возврат:
        //     Значение указанного атрибута. Этот метод не изменяет позицию средства чтения.
        //
        // Исключения:
        //   T:System.ArgumentOutOfRangeException:
        //     i выходит за пределы диапазона. Оно должно быть неотрицательным и меньшим, чем
        //     размер коллекции атрибутов.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAttribute(int i);
        //
        // Сводка:
        //     Вызовы System.Xml.XmlReader.MoveToContent и проверяет, является ли текущий узел
        //     содержимого открывающим тегом или пустым тегом элемента.
        //
        // Возврат:
        //     true Если System.Xml.XmlReader.MoveToContent находит открывающий тег или пустой
        //     тег элемента; false Если узел типом, отличным от XmlNodeType.Element найден.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     Неверный XML встречается во входном потоке.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool IsStartElement();
        //
        // Сводка:
        //     Когда переопределено в производном классе, разрешает префикс пространства имен
        //     в области видимости текущего элемента.
        //
        // Параметры:
        //   prefix:
        //     Префикс, для которого требуется разрешить URI пространства имен. Чтобы сопоставить
        //     пространство имен по умолчанию, необходимо передать пустую строку.
        //
        // Возврат:
        //     URI пространства имен, которое отображает префикс, или значение null, если соответствующий
        //     префикс не найден.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string LookupNamespace([MarshalAs(UnmanagedType.LPWStr)] string prefix);
        //
        // Сводка:
        //     Когда переопределено в производном классе, переходит к атрибуту с указанным индексом.
        //
        // Параметры:
        //   i:
        //     Индекс атрибута.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        //
        //   T:System.ArgumentOutOfRangeException:
        //     Параметр имеет отрицательное значение.
        void MoveToAttribute(int i);
        //
        // Сводка:
        //     Проверяет, является ли текущий узел содержимого (текст пустое пространство, CDATA,
        //     Element, EndElement, EntityReference, или EndEntity) узла. Если узел не является
        //     узлом содержимого, средство чтения пропускает этот узел и переходит к следующему
        //     узлу содержимого или в конец файла. Пропускаются узлы следующих типов: ProcessingInstruction,
        //     DocumentType, Comment, Whitespace и SignificantWhitespace.
        //
        // Возврат:
        //     System.Xml.XmlReader.NodeType Текущего узла, найденного с помощью метода или
        //     XmlNodeType.None Если средство чтения достигло конца потока входных данных.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     Во входном потоке обнаружен неверный XML-код.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U4)]
        XmlNodeType MoveToContent();
        //
        // Сводка:
        //     Когда переопределено в производном классе, переходит к элементу, содержащему
        //     текущий узел атрибута.
        //
        // Возврат:
        //     Значение true, если средство чтения находится на атрибуте (средство чтения перемещается
        //     к элементу с этим атрибутом); в противном случае — false (позиция средства чтения
        //     не изменяется).
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool MoveToElement();
        //
        // Сводка:
        //     Когда переопределено в производном классе, переходит к первому атрибуту.
        //
        // Возврат:
        //     Значение true, если атрибут существует (средство чтения перемещается к первому
        //     атрибуту); в противном случае — false (позиция средства чтения не изменяется).
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool MoveToFirstAttribute();
        //
        // Сводка:
        //     Когда переопределено в производном классе, переходит к следующему атрибуту.
        //
        // Возврат:
        //     Значение true, если присутствует следующий атрибут; значение false, если другие
        //     атрибуты отсутствуют.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool MoveToNextAttribute();
        //
        // Сводка:
        //     Когда переопределено в производном классе, считывает из потока следующий узел.
        //
        // Возврат:
        //     true, если считывание узла прошло успешно. В противном случае — false.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     Произошла ошибка при синтаксическом анализе XML.
        //
        //   T:System.InvalidOperationException:
        //     Метод System.Xml.XmlReader вызван перед завершением предыдущей асинхронной операции.
        //     В этом случае возникает исключение System.InvalidOperationException с сообщением
        //     "Асинхронная операция уже выполняется".
        [return: MarshalAs(UnmanagedType.U1)]
        bool Read();
        //
        // Сводка:
        //     При переопределении в производном классе разбирает значение атрибута в один или
        //     несколько Text, EntityReference, или EndEntity узлов.
        //
        // Возврат:
        //     Значение true, если присутствуют возвращаемые узлы. Значение false, если средство
        //     чтения не расположено на узле атрибута при первом вызове или все значения атрибута
        //     считаны. Пустой атрибут (например, misc="") возвращает значение true с отдельным
        //     узлом, имеющим значение String.Empty.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.U1)]
        bool ReadAttributeValue();
        //
        // Сводка:
        //     Считывает содержимое текста в текущей позиции как System.Object.
        //
        // Возврат:
        //     Текстовое содержимое как самый подходящий объект CLR.
        //
        // Исключения:
        //   T:System.InvalidCastException:
        //     Попытка приведения типов не является допустимым.
        //
        //   T:System.FormatException:
        //     Недопустимый формат строки.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.Interface)]
        object ReadContentAsObject();
        //
        // Сводка:
        //     Считывает содержимое текста в текущей позиции как System.String объект.
        //
        // Возврат:
        //     Текстовое содержимое в виде System.String объекта.
        //
        // Исключения:
        //   T:System.InvalidCastException:
        //     Попытка приведения типов не является допустимым.
        //
        //   T:System.FormatException:
        //     Недопустимый формат строки.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReadContentAsString();
        //
        // Сводка:
        //     Считывает текущий элемент и возвращает содержимое в виде System.Object.
        //
        // Возврат:
        //     Упакованный объект CLR наиболее подходящего типа. System.Xml.XmlReader.ValueType
        //     Свойство определяет соответствующий тип среды CLR. Если содержимое типизировано
        //     как тип списка, этот метод возвращает массив упакованных объектов соответствующего
        //     типа.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Не находится на элементе.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        //
        //   T:System.Xml.XmlException:
        //     Текущий элемент содержит дочерние элементы. -или- Содержимое элемента нельзя
        //     преобразовать в требуемый тип
        //
        //   T:System.ArgumentNullException:
        //     Метод вызывается с null аргументы.
        [return: MarshalAs(UnmanagedType.Interface)]
        object ReadElementContentAsObject();
        //
        // Сводка:
        //     Считывает текущий элемент и возвращает содержимое в виде System.String объекта.
        //
        // Возврат:
        //     Содержимое элемента в виде System.String объекта.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Не находится на элементе.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        //
        //   T:System.Xml.XmlException:
        //     Текущий элемент содержит дочерние элементы. -или- Не удается преобразовать содержимое
        //     элемента System.String объекта.
        //
        //   T:System.ArgumentNullException:
        //     Метод вызывается с null аргументы.
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReadElementContentAsString();
        //
        // Сводка:
        //     Проверяет, является ли текущий узел содержимого закрывающим тегом, и позиционирует
        //     средство чтения на следующий узел.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     Текущий узел не является закрывающим тегом или если неверный XML встречается
        //     во входном потоке.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        void ReadEndElement();
        //
        // Сводка:
        //     Когда переопределено в производном классе, считывает как строку все содержимое,
        //     включая разметку.
        //
        // Возврат:
        //     Все содержимое XML-кода в текущем узле, включая разметку. Если текущий узел не
        //     имеет дочерних узлов, возвращается пустая строка. Если текущий узел не является
        //     элементом или атрибутом, возвращается пустая строка.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     XML-код неверен или произошла ошибка при разборе XML.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReadInnerXml();
        //
        // Сводка:
        //     Когда переопределено в производном классе, считывает содержимое, включая разметку,
        //     представляющую этот узел и все его дочерние узлы.
        //
        // Возврат:
        //     Если средство чтения позиционировано на узел элемента или атрибута, данный метод
        //     возвращает все содержимое XML текущего узла и всех его дочерних узлов, включая
        //     разметку; в противном случае возвращается пустая строка.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     XML-код неверен или произошла ошибка при разборе XML.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string ReadOuterXml();
        //
        // Сводка:
        //     Проверяет, является ли текущий узел элементом и перемещает модуль чтения к следующему
        //     узлу.
        //
        // Исключения:
        //   T:System.Xml.XmlException:
        //     Во входном потоке обнаружен неправильный XML.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        void ReadStartElement();
        //
        // Сводка:
        //     Возвращает новый XmlReader экземпляр, который может использоваться для считывания
        //     текущего узла и всех его потомков.
        //
        // Возврат:
        //     Новый экземпляр средства чтения XML, равным System.Xml.ReadState.Initial. Вызов
        //     System.Xml.XmlReader.Read метод помещает новый модуль чтения на узел, который
        //     был текущим перед вызовом System.Xml.XmlReader.ReadSubtree метод.
        //
        // Исключения:
        //   T:System.InvalidOperationException:
        //     Средство чтения XML не находится на элементе при вызове этого метода.
        //
        //   T:System.InvalidOperationException:
        //     System.Xml.XmlReader Метод был вызван до завершения предыдущей асинхронной операции.
        //     В этом случае System.InvalidOperationException исключение с сообщением «асинхронная
        //     операция уже выполняется.»
        IXmlReader ReadSubtree();
    }
    public class TXmlReader : IXmlReader
    {
        private readonly XmlReader r;
        public TXmlReader(XmlReader r) => this.r = r;
        bool IXmlReader.HasAttributes() => r.HasAttributes;
        string IXmlReader.XmlLang() => r.XmlLang;
        XmlSpace IXmlReader.XmlSpace() => r.XmlSpace;
        bool IXmlReader.IsDefault() => r.IsDefault;
        bool IXmlReader.IsEmptyElement() => r.IsEmptyElement;
        string IXmlReader.BaseURI() => r.BaseURI;
        string IXmlReader.Value() => r.Value;
        bool IXmlReader.HasValue() => r.HasValue;
        string IXmlReader.Prefix() => r.Prefix;
        XmlNodeType IXmlReader.NodeType() => r.NodeType;
        string IXmlReader.Name() => r.Name;
        int IXmlReader.Depth() => r.Depth;
        IComXmlSchemaInfo IXmlReader.SchemaInfo() => new TXmlSchemaInfo(r.SchemaInfo);
        int IXmlReader.AttributeCount() => r.AttributeCount;
        string IXmlReader.LocalName() => r.LocalName;
        string IXmlReader.NamespaceURI() => r.NamespaceURI;
        ReadState IXmlReader.ReadState() => r.ReadState;
        bool IXmlReader.EOF() => r.EOF;

        string IXmlReader.GetAttribute(int i) => r.GetAttribute(i);
        bool IXmlReader.IsStartElement() => r.IsStartElement();
        string IXmlReader.LookupNamespace(string prefix) => r.LookupNamespace(prefix);
        void IXmlReader.MoveToAttribute(int i) => r.MoveToAttribute(i);
        XmlNodeType IXmlReader.MoveToContent() => r.MoveToContent();
        bool IXmlReader.MoveToElement() => r.MoveToElement();
        bool IXmlReader.MoveToFirstAttribute() => r.MoveToFirstAttribute();
        bool IXmlReader.MoveToNextAttribute() => r.MoveToNextAttribute();
        bool IXmlReader.Read() => r.Read();
        bool IXmlReader.ReadAttributeValue() => r.ReadAttributeValue();
        object IXmlReader.ReadContentAsObject() => r.ReadContentAsObject();
        string IXmlReader.ReadContentAsString() => r.ReadContentAsString();
        object IXmlReader.ReadElementContentAsObject() => r.ReadElementContentAsObject();
        string IXmlReader.ReadElementContentAsString() => r.ReadElementContentAsString();
        void IXmlReader.ReadEndElement() => r.ReadEndElement();
        string IXmlReader.ReadInnerXml() => r.ReadInnerXml();
        string IXmlReader.ReadOuterXml() => r.ReadOuterXml();
        void IXmlReader.ReadStartElement() => r.ReadStartElement();
        IXmlReader IXmlReader.ReadSubtree() => new TXmlReader(r.ReadSubtree());
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
        IXmlSchemaValidator Validator([MarshalAs(UnmanagedType.LPWStr)] string FileName, out IXmlReader IReader);

        IXmlSchemaObjectCollection DerivedFrom(IXmlSchemaType baseType);
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
        IXmlSchemaValidator IXmlSchemaSet.Validator(string FileName, out IXmlReader IReader)
        {
            XmlReader reader = XmlReader.Create(FileName);
            IReader = new TXmlReader(reader);
            return new TXmlSchemaValidator(reader.NameTable, s, new XmlNamespaceManager(reader.NameTable), XmlSchemaValidationFlags.None);
        }
        IXmlSchemaObjectCollection IXmlSchemaSet.DerivedFrom(IXmlSchemaType baseType)
        {
            XmlSchemaObjectCollection res = new XmlSchemaObjectCollection();
            XmlSchemaType bt = (XmlSchemaType)(baseType as IXmlSchemaObject).XmlObject();
            void AddDetived(XmlSchema sc)
            {
                foreach (XmlSchemaType st in sc.SchemaTypes.Values)
                {
                    if (XmlSchemaType.IsDerivedFrom(st, bt, XmlSchemaDerivationMethod.None))
                    {
                        if (!res.Contains(st) && (st != bt)) res.Add(st);
                    }
                }
            }
            foreach (XmlSchema sx in s.Schemas())
            {
                AddDetived(sx);
                foreach (XmlSchemaExternal se in sx.Includes)
                {
                    AddDetived(se.Schema);
                }
            }
            return new TXmlSchemaObjectCollection(res);
        }
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void GetXmlSchemaSet(out IXmlSchemaSet OutD) => OutD = new TXmlSchemaSet();

    }
}


