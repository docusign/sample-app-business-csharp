namespace DocuSign.MyBusiness.Infrustructure.Model
{
    public class DataSourceItem
    {
        public DataSourceItem()
        {
        }

        public DataSourceItem(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
