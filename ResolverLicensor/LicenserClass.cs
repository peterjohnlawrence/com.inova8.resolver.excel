namespace com.inova8.resolver
{
    using System;
    using System.Text;
    using System.Windows.Forms;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.IO;
    using System.Xml;
    using System.Security.Cryptography;

    public class LicenserClass
    {
        public LicenseInfo licenseInformation = new LicenseInfo();
        public const int trialPeriod = 20;
        public const int maxCells = 250;
        public enum LicenseType
        {
            Trial,
            Full,
            Null
        } ;

        public struct FeatureInfo
        {
            public string featureName;
            public bool timeDependent;
            public DateTime expiration;
            public int maxCount;
            public FeatureInfo(string FeatureName, bool TimeDependent, DateTime Expiration, int MaxCount)
            {
                featureName = FeatureName;
                timeDependent = TimeDependent;
                expiration = Expiration;
                maxCount = MaxCount;
            }
            public void AppendFeature(XmlDocument doc, XmlElement elem)
            {
                XmlElement child = doc.CreateElement("Feature");
                elem.AppendChild(child);
                XmlAttribute attr = doc.CreateAttribute("Name");
                attr.Value = featureName;
                child.Attributes.Append(attr);

                attr = doc.CreateAttribute("IsTimeDependent");
                attr.Value = XmlConvert.ToString(timeDependent);
                child.Attributes.Append(attr);

                if (timeDependent)
                {
                    attr = doc.CreateAttribute("Expiration");
                    attr.Value = XmlConvert.ToString(expiration, XmlDateTimeSerializationMode.Local);
                    child.Attributes.Append(attr);
                }

                attr = doc.CreateAttribute("MaxCount");
                attr.Value = XmlConvert.ToString(maxCount);
                child.Attributes.Append(attr);
            }
        };
        public struct LicenseInfo
        {
            public string product;
            public LicenseType type;
            public string computerID;
            public DateTime issued;
            public string passCode;
        };
        public enum licenseValidity
        {
            valid,
            trial,
            missing,
            mismatchSignature,
            expired
        };
        public static string GetComputerId()
        {
            string result = "";

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // look for computer network mac-address
                NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
                PhysicalAddress address = ifaces[0].GetPhysicalAddress();
                byte[] byteAddr = address.GetAddressBytes();
                // Convert it to hex digits separated with "-" sign
                for (int i = 0; i < byteAddr.Length; i++)
                {
                    result += byteAddr[i].ToString("X2");
                    if (i != byteAddr.Length - 1)
                    {
                        result += "-";
                    }
                }
            }
            else
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.DriveType == DriveType.Fixed)
                    {
                        result = drive.VolumeLabel;
                        break;
                    }
                }
            }

            return result;
        }
        public bool handleLicense()
        {
            licenseValidity validity = IsValid(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\inova8\\Resolver\\Resolver.xml", "INOVA8");

            com.inova8.resolver.frmRegister register = new com.inova8.resolver.frmRegister(this);
            switch (validity)
            {
                case licenseValidity.valid:
                    return true;
                case licenseValidity.trial:
                    MessageBox.Show("You have " + licenseInformation.issued.AddDays(trialPeriod).Subtract(System.DateTime.Today).Days + " days remaining of your trial", "Resolver Trial License");
                    return true;
                case licenseValidity.expired:
                    MessageBox.Show("The trial has now expired. Why not buy a full license?", "Resolver Trial License");
                    return false;
                case licenseValidity.missing:
                    MessageBox.Show("Resolver is unlicensed. You will be limited to problems defined in " + maxCells + " spreadsheet cells", "Resolver Trial License");
                    return true;
                case licenseValidity.mismatchSignature:
                    licenseInformation.type = LicenseType.Null;
                    register.ShowDialog();
                    if (licenseInformation.type == LicenseType.Null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }

        public licenseValidity IsValid(string licensePath, string passCode)
        {
            //return licenseValidity.valid;

            if (DateTime.Compare(DateTime.Today, DateTime.Parse("2012-12-31T00:00:00-05:00")) < 0)
            {
                return licenseValidity.valid;
            }
            else
            {
                string licenseSignature;
                licenseInformation = GetLicenseFromFile(licensePath, passCode, out licenseSignature);
                if (licenseInformation.product == null)
                {
                    licenseInformation.product = null;
                    licenseInformation.type = LicenseType.Null;
                    return licenseValidity.missing;
                }
                licenseInformation.computerID = LicenserClass.GetComputerId();
                string signature = CreateSignature(licenseInformation);
                if (signature != licenseSignature)
                {
                    licenseInformation.product = null;
                    licenseInformation.type = LicenseType.Null;
                    return licenseValidity.mismatchSignature;
                }
                if ((DateTime.Now > licenseInformation.issued.AddDays(trialPeriod)) & licenseInformation.type == LicenseType.Trial)
                {
                    licenseInformation.product = null;
                    licenseInformation.type = LicenseType.Null;
                    return licenseValidity.expired;
                }
                if (licenseInformation.type == LicenseType.Trial)
                {
                    return licenseValidity.trial;
                }
                return licenseValidity.valid;
            }
        }

        public LicenseType ProduceSingleLicense(string signature)
        {
            licenseInformation.product = "Resolver";
            licenseInformation.type = LicenseType.Full;
            licenseInformation.computerID = GetComputerId();
            licenseInformation.issued = System.DateTime.MinValue; // System.DateTime.Today.AddDays(20);
            licenseInformation.passCode = "INOVA8";

            if (signature != CreateSignature(licenseInformation))
            {   //try trial license
                licenseInformation.type = LicenseType.Trial;
                int i = 0;
                string tempSignature = "";
                do
                {
                    licenseInformation.issued = System.DateTime.Today.AddDays(-i);
                    tempSignature = CreateSignature(licenseInformation);
                    i += 1;

                } while ((i < 30) && (signature != tempSignature));
                if (signature != tempSignature)
                {
                    licenseInformation.type = LicenseType.Null;
                    return LicenseType.Null;
                }
            }

            XmlDocument doc = new XmlDocument();

            XmlElement elem = doc.CreateElement("Resolver");
            doc.AppendChild(elem);

            XmlElement child = doc.CreateElement("LicenseType");
            child.InnerText = licenseInformation.type.ToString();//licenseType.ToString();
            elem.AppendChild(child);

            child = doc.CreateElement("ComputerId");
            child.InnerText = licenseInformation.computerID;//computerId;
            elem.AppendChild(child);

            child = doc.CreateElement("Issued");
            child.InnerText = XmlConvert.ToString(licenseInformation.issued, XmlDateTimeSerializationMode.Local);
            elem.AppendChild(child);

            child = doc.CreateElement("Signature");

            child.InnerText = signature;
            elem.AppendChild(child);
            string rootPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            try
            {
                System.IO.Directory.CreateDirectory(rootPath + "\\inova8");
            }
            catch { }
            try
            {
                System.IO.Directory.CreateDirectory(rootPath + "\\inova8\\Resolver");
            }
            catch { }

            try
            {
                doc.Save(rootPath + "\\inova8\\Resolver\\Resolver.xml");
            }
            catch
            {
                MessageBox.Show("Could not write license to '" + rootPath + "'. Please check with admistratives privileges on this directory. Resolver need sto write its license key here.", "Resolver Registration");
            }
            return licenseInformation.type;
        }
        LicenseInfo GetLicenseFromFile(string licensePath, string passCode, out string signature)
        {
            LicenseInfo licenseInformation = new LicenseInfo();
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(licensePath);

                licenseInformation.product = xdoc.DocumentElement.Name;// "Resolver";

                licenseInformation.type =
                    (LicenseType)Enum.Parse(
                        typeof(LicenseType),
                        xdoc.DocumentElement["LicenseType"].InnerText,
                        true);
                licenseInformation.computerID = xdoc.DocumentElement["ComputerId"].InnerText;
                licenseInformation.issued = XmlConvert.ToDateTime(xdoc.DocumentElement["Issued"].InnerText, XmlDateTimeSerializationMode.Local);
                licenseInformation.passCode = passCode;
                signature = xdoc.DocumentElement["Signature"].InnerText;
            }
            catch
            {
                licenseInformation.product = null;
                signature = "";
            }
            return licenseInformation;
        }
        public string CreateSignature(LicenseInfo licenseInformation)
        {
            SHA384Managed shaM = new SHA384Managed();
            byte[] data;

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((string)licenseInformation.product);
            bw.Write((int)licenseInformation.type);
            bw.Write(licenseInformation.computerID);
            bw.Write(licenseInformation.passCode);
            bw.Write(XmlConvert.ToString(licenseInformation.issued, XmlDateTimeSerializationMode.Local));

            int nLen = (int)ms.Position + 1;
            bw.Close();
            ms.Close();
            data = ms.GetBuffer();

            data = shaM.ComputeHash(data, 0, nLen);

            string result = "";
            foreach (byte dbyte in data)
            {
                result += dbyte.ToString("X2");
            }
            return result;
        }


        public class LicenseException : System.Exception
        {
            public LicenseException(string message)
                : base(message)
            {
            }
        }
    }
}
