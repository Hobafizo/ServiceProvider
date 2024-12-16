namespace ServiceProvider.Data
{
    public enum NavItemPos
    {
        Center,
        Right
    }

    public class NavItem
    {
        private string? m_actionname;

        public string ItemName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName
        {
            get
            {
                return m_actionname ?? string.Empty;
            }
            set
            {
                m_actionname = value;
            }
        }

        public NavItemPos Position { get; set; }
        public bool Selected { get; set; }

        public NavItem(string itemName, string controllerName, string? actionName = null, NavItemPos pos = NavItemPos.Center, bool selected = false)
        {
            ItemName = itemName;
            ControllerName = controllerName;
            m_actionname = actionName;
            Position = pos;
            Selected = selected;
        }

        public string SelectionProperties
        {
            get
            {
                return Selected ? "selected" : "";
            }
        }
    }
}
