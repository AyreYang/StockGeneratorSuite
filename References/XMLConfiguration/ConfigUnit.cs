using System;
using System.Collections.Generic;
using System.IO;

namespace XMLConfiguration
{
    public class ConfigUnit
    {
        public ConfigUnit RootUnit = null;
        public ConfigUnit ParentUnit = null;

        private string ms_tag = string.Empty;
        public string Tag
        {
            get { return this.ms_tag; }
            set { this.ms_tag = value; }
        }

        private string ms_id = string.Empty;
        public string ID
        {
            get { return this.ms_id; }
            set { this.ms_id = value; }
        }

        private CONFIG_UNIT_TYPE mcut_type = CONFIG_UNIT_TYPE.LIST;
        public CONFIG_UNIT_TYPE Type
        {
            get { return this.mcut_type; }
            set { this.mcut_type = value; }
        }

        private Dictionary<string, string> mdic_properties = new Dictionary<string, string>();
        public Dictionary<string, string> Properties
        {
            get { return this.mdic_properties; }
            //set { this.mdic_properties = value; }
        }

        private Object mobj_value = null;
        public Object Value
        {
            get { return this.mobj_value; }
            set { this.mobj_value = value; }
        }

        public ConfigUnit GetUnit(DirectoryInfo adi_path, ref string as_msg)
        {
            if (adi_path == null)
            {
                as_msg = Messages.PATH_NULL;
                return null;
            }

            ConfigUnit lcu_unit = null;
            ConfigUnit lcu_from = null;
            string ls_root_mark = @".." + Path.DirectorySeparatorChar;
            string ls_parent_mark = @"." + Path.DirectorySeparatorChar;
            string ls_path_temp = adi_path.ToString().Replace(ls_root_mark, string.Empty).Replace(ls_parent_mark, string.Empty);
            DirectoryInfo ldi_path = new DirectoryInfo(ls_path_temp);
            string[] ls_path = ls_path_temp.Split(Path.DirectorySeparatorChar);

            if (adi_path.ToString().StartsWith(ls_root_mark))
            {
                lcu_from = this.RootUnit;
                if (lcu_from == null) lcu_from = this;
            }
            else if (adi_path.FullName.StartsWith(ls_parent_mark))
            {
                lcu_from = this.ParentUnit;
                if (lcu_from == null) lcu_from = this;
            }
            else
            {
                lcu_from = this;
            }

            Dictionary<string, ConfigUnit> ldt_values = null;
            List<ConfigUnit> lst_values = null;
            foreach (string ls_temp in ls_path)
            {
                if (lcu_from.Value == null) break;

                switch (lcu_from.Type)
                {
                    case CONFIG_UNIT_TYPE.STRING:
                        lcu_unit = lcu_from;
                        lcu_from = null;
                        break;

                    case CONFIG_UNIT_TYPE.DICT:
                        ldt_values = lcu_from.Value as Dictionary<string, ConfigUnit>;
                        if (ldt_values.ContainsKey(ls_temp))
                        {
                            lcu_from = ldt_values[ls_temp];
                        }
                        else
                        {
                            lcu_from = null;
                        }
                        break;

                    case CONFIG_UNIT_TYPE.LIST:
                        lst_values = lcu_from.Value as List<ConfigUnit>;
                        lcu_from = null;
                        foreach (ConfigUnit lcu_temp in lst_values)
                        {
                            if (lcu_temp.ID == ls_temp)
                            {
                                lcu_from = lcu_temp;
                            }
                        }
                        break;
                }

                if (lcu_from == null) break;
            }

            if (lcu_from != null) lcu_unit = lcu_from;

            if (lcu_unit == null || lcu_unit.ID != ldi_path.Name)
            {
                return null;
            }
            else
            {
                return lcu_unit;
            }

        }

        public List<ConfigUnit> GetValueList()
        {
            List<ConfigUnit> llst_value = null;
            if (this.Value is List<ConfigUnit>)
            {
                llst_value = this.Value as List<ConfigUnit>;
            }

            return llst_value;
        }

        public Dictionary<string, ConfigUnit> GetValueDict()
        {
            Dictionary<string, ConfigUnit> ldt_value = null;
            if (this.Value is Dictionary<string, ConfigUnit>)
            {
                ldt_value = this.Value as Dictionary<string, ConfigUnit>;
            }

            return ldt_value;
        }

        public string GetValueString()
        {
            string ls_value = null;
            if (this.Value is string)
            {
                ls_value = this.Value as string;
            }

            return ls_value;
        }

    }
}
