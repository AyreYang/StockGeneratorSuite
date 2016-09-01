using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XMLConfiguration
{
    public class Configuration
    {
        public ConfigUnit Config = null;

        public Configuration()
        {
            //Config = new ConfigUnit();
        }

        public ConfigUnit SearchUnit(DirectoryInfo adi_path, ref string as_msg)
        {
            return Config.GetUnit(adi_path, ref as_msg);
        }

        public int Load(FileInfo afi_config, ref string as_msg)
        {
            Config = new ConfigUnit();
            return LoadXML(afi_config, ref Config, ref as_msg);
        }

        private int LoadXML(FileInfo afi_xml, ref ConfigUnit acu_config, ref string as_msg)
        {
            string ls_value = string.Empty;
            if (afi_xml == null)
            {
                as_msg = Messages.XML_FILE_NULL;
                return -1;
            }
            if (!File.Exists(afi_xml.FullName))
            {
                as_msg = string.Format(Messages.XML_FILE_UNEXIST, afi_xml.FullName);
                return -1;
            }
            XmlDocument lxml_doc = new XmlDocument();
            lxml_doc.Load(afi_xml.FullName);
            XmlNode lxml_config = lxml_doc.SelectSingleNode("/Config");
            return LoadXMLNode(lxml_config, null, null, ref acu_config, ref as_msg);
        }


        private int LoadXMLNode(XmlNode lxml_source, ConfigUnit acu_root, ConfigUnit acu_parent, ref ConfigUnit acu_config, ref string as_msg)
        {
            int li_ret = 0;

            Dictionary<string, ConfigUnit> ldt_values = null;
            List<ConfigUnit> lst_values = null;
            ConfigUnit lcu_child;

            try
            {
                acu_config.Tag = lxml_source.Name;
                acu_config.Type = CONFIG_UNIT_TYPE.LIST;
                acu_config.RootUnit = acu_root;
                acu_config.ParentUnit = acu_parent;
                acu_config.Properties.Clear();
                foreach (XmlAttribute lxa_temp in lxml_source.Attributes)
                {
                    if (!acu_config.Properties.ContainsKey(lxa_temp.Name))
                    {
                        acu_config.Properties.Add(lxa_temp.Name, lxa_temp.Value);
                    }
                    else
                    {
                        continue;
                    }

                    if (lxa_temp.Name.Trim().ToUpper().Equals("ID"))
                    {
                        acu_config.ID = lxa_temp.Value.Trim();
                        continue;
                    }
                    if (lxa_temp.Name.Trim().ToUpper().Equals("VALUE"))
                    {
                        acu_config.Value = lxa_temp.Value.Trim();
                        continue;
                    }

                    if (!lxa_temp.Name.Trim().ToUpper().Equals("TYPE")) continue;

                    if (string.IsNullOrEmpty(lxa_temp.Value)) continue;

                    if (lxa_temp.Value.ToUpper().Equals(CONFIG_UNIT_TYPE.LIST.ToString()))
                    {
                        acu_config.Type = CONFIG_UNIT_TYPE.LIST;
                    }
                    else if (lxa_temp.Value.ToUpper().Equals(CONFIG_UNIT_TYPE.DICT.ToString()))
                    {
                        acu_config.Type = CONFIG_UNIT_TYPE.DICT;
                    }
                    else if (lxa_temp.Value.ToUpper().Equals(CONFIG_UNIT_TYPE.STRING.ToString()))
                    {
                        acu_config.Type = CONFIG_UNIT_TYPE.STRING;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (string.IsNullOrEmpty(acu_config.ID)) acu_config.ID = acu_config.Tag;
                if (lxml_source.ChildNodes == null || lxml_source.ChildNodes.Count <= 0)
                {
                    return 1;
                }

                switch (lxml_source.FirstChild.NodeType)
                {
                    case XmlNodeType.Text:
                        acu_config.Type = CONFIG_UNIT_TYPE.STRING;
                        acu_config.Value = lxml_source.InnerText.StartsWith("{AppPath}") ? lxml_source.InnerText.Replace(@"{AppPath}\", AppDomain.CurrentDomain.SetupInformation.ApplicationBase) : lxml_source.InnerText;
                        li_ret = 1;
                        break;

                    case XmlNodeType.Element:
                        if (acu_config.Type == CONFIG_UNIT_TYPE.STRING) acu_config.Type = CONFIG_UNIT_TYPE.LIST;
                        foreach (XmlNode lxml_node in lxml_source.ChildNodes)
                        {
                            lcu_child = new ConfigUnit();
                            li_ret = LoadXMLNode(lxml_node, (acu_root == null ? acu_config : acu_root), acu_config, ref lcu_child, ref as_msg);
                            if (li_ret != 1) return li_ret;
                            switch (acu_config.Type)
                            {
                                case CONFIG_UNIT_TYPE.LIST:
                                    if (lst_values == null) lst_values = new List<ConfigUnit>();
                                    lst_values.Add(lcu_child);
                                    break;
                                case CONFIG_UNIT_TYPE.DICT:
                                    if (ldt_values == null) ldt_values = new Dictionary<string, ConfigUnit>();
                                    if (string.IsNullOrEmpty(lcu_child.ID))
                                    {
                                        as_msg = string.Format(Messages.TAG_ID_NULL_EMPTY, lcu_child.Tag);
                                        return 0;
                                    }
                                    if (ldt_values.ContainsKey(lcu_child.ID))
                                    {
                                        as_msg = string.Format(Messages.TAG_ID_EXISTED, lcu_child.Tag);
                                        return 0;
                                    }
                                    ldt_values.Add(lcu_child.ID, lcu_child);
                                    break;
                            }
                        }

                        switch (acu_config.Type)
                        {
                            case CONFIG_UNIT_TYPE.LIST:
                                acu_config.Value = lst_values;
                                break;
                            case CONFIG_UNIT_TYPE.DICT:
                                acu_config.Value = ldt_values;
                                break;
                        }

                        li_ret = 1;

                        break;

                    default:
                        break;
                }

                return li_ret;


            }
            catch (System.Exception err)
            {
                as_msg = err.Message;
                return -1;
            }
        }


    }
}
