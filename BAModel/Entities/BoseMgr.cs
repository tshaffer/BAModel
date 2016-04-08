using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace BAModel
{
    public class BoseMgr
    {
        public static bool BoseProductsExist { get; set; }

        private static List<BoseProduct> _boseProducts = new List<BoseProduct>();
        public static List<BoseProduct> BoseProducts
        {
            get { return _boseProducts; }
        }

        public static string BoseProductsPath { get; set; }

        public static bool TripleUSBSupported
        {
            get
            {
                if (!BrightSignModelMgr.Model960Enabled) return false;
                if (BrightSignModelMgr.CurrentBrightSignModel == null) return false;
                bool supported = 
                ( (_boseProducts.Count > 0) &&
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.HD920) ||
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.HD970) ||
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.HD922) ||
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.HD972) ||
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.A915)) ||
                    (BrightSignModelMgr.CurrentBrightSignModel.Model == BrightSignModel.ModelType.HD917)
                    ;
                return supported;
            }
        }

        public static void Initialize()
        {
            BoseProductsExist = false;

            try
            {
                Uri newUri = new Uri("templates/BoseProducts.xml", UriKind.Relative);
                string boseProductsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, newUri.ToString());
                Trace.WriteLine("boseProductsPath is " + boseProductsPath);

                if (File.Exists(boseProductsPath))
                {
                    StreamReader sr = new StreamReader(boseProductsPath);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(sr);
                    ParseBoseProducts(doc);

                    BoseProductsExist = true;
                    BoseProductsPath = boseProductsPath;
                }
            }
            catch (Exception ex)
            {
//                App.myTraceListener.Assert(false, ex.ToString());
            }
        }

        private static void ParseBoseProducts(XmlDocument doc)
        {
            XmlElement docElement = doc.DocumentElement;
            if (docElement.Name != "BoseProducts")
            {
                Trace.WriteLine("Missing BoseProducts docElement name");
                return;
            }

            if (!docElement.HasAttributes)
            {
                Trace.WriteLine("docElement has no attributes");
                return;
            }

            if (docElement.Attributes[0].Name != "version")
            {
                Trace.WriteLine("missing version attribute");
                return;
            }

            XmlNodeList productsXML = doc.GetElementsByTagName("product");

            foreach (XmlElement productXML in productsXML)
            {
                BoseProduct boseProduct = BoseProduct.ReadXml(productXML);
                if (boseProduct != null)
                {
                    _boseProducts.Add(boseProduct);
                }
            }
        }

        public static BoseProduct GetBoseProduct(string productName)
        {
            BoseProduct boseProduct = null;

            foreach (BoseProduct existingBoseProduct in _boseProducts)
            {
                if (productName == existingBoseProduct.ProductName)
                {
                    boseProduct = existingBoseProduct;
                    break;
                }
            }
            return boseProduct;
        }

        public static BoseProduct GetLegacyProduct()
        {
            return GetBoseProduct("LegacyProduct");
        }
    }

    public class BoseProductInPresentation
    {
        public string ProductName { get; set; }
        public string Port { get; set; }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            BoseProductInPresentation bpip = (BoseProductInPresentation)obj;

            return bpip.ProductName == this.ProductName && bpip.Port == this.Port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static BoseProductInPresentation ReadXml(XmlReader reader)
        {
            BoseProductInPresentation bpip = new BoseProductInPresentation();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "productName":
                        bpip.ProductName = reader.ReadString();
                        break;
                    case "port":
                        bpip.Port = reader.ReadString();
                        break;
                }
            }

            return bpip;
        }
    }
}
