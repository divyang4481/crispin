﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using crispin.Helpers;

namespace crispin
{
    public class HtmlReportConverter : IReportConverter
    {
        XslCompiledTransform _xslt = XsltLoader.LoadFromAssembly("crispin.xslt.XrptToHtml.xslt");

        public void ReplaceXslt(XslCompiledTransform newXslt)
        {
            _xslt = newXslt;
        }

        public string ConvertToString(string xrpt)
        {
            var xml = new XmlDocument();
            xml.LoadXml(StripByteOrderMark.Strip(xrpt));

            using (var writer = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.Formatting = Formatting.Indented;

                xmlWriter.WriteStartDocument();
                _xslt.Transform(xml, null, xmlWriter, new XmlUrlResolver());

                return writer.ToString()
                    .Replace(Convert.ToString((char)160), "&nbsp;");
            }
        }

        public byte[] ConvertToBuffer(string xrpt, string reportName)
        {
            return Encoding.UTF8.GetBytes(ConvertToString(xrpt));
        }
    }
}
