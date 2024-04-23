using Microsoft.AspNetCore.Html;
using System.Text;

namespace Hotels.Utilities
{
    public static class HtmlUtils
    {
        public static IHtmlContent GetCheckboxInput(string id, string display, bool value)
        {
            string idView = "View" + id;
            StringBuilder text = new StringBuilder();
            string checkedHtml = value ? "checked" : string.Empty;
            text.Append($@"
                <div class=""form-check"" style=""padding-top: 30px;"">
                    <input class=""form-check-input"" type=""checkbox"" id=""{idView}"" {checkedHtml}>
                    <input type=""text"" name=""{id}"" id=""{id}"" hidden>
                    <label class=""form-check-label"" for=""flexCheckChecked"">
                        {display}
                    </label>
                </div>
                
                <script>

                const checkbox = document.getElementById('{idView}');
                const outputInput = document.getElementById('{id}');
                outputInput.value = '{value}';

                checkbox.addEventListener('change', function() {{
                    outputInput.value = this.checked ? 'true' : 'false';
                }});

                </script>
            ");

            return new HtmlString(text.ToString());
        }

        public static IHtmlContent GetInputConfiguration(string module, string name, string value, string type)
        {
            StringBuilder text = new StringBuilder();
            switch (type.ToLower())
            {
                case "bool":
                        bool.TryParse(value, out bool boolValue);
                        return GetCheckboxInput($"{module}-{name}", name, boolValue);
                    break;

                case "int":
                    text.Append($@"
                        <label for=""{module}-{name}"">{name}:</label>
                        <input name=""{module}-{name}"" data-validation=""filtrar"" class=""form-control"" type=""number"" value=""{value}"">
                    ");
                    break;

                default:
                    text.Append($@"
                        <label for=""{module}-{name}"">{name}:</label>
                        <input name=""{module}-{name}"" data-validation=""filtrar"" class=""form-control"" type=""text"" value=""{value}"">
                    ");
                    break;
            }
            return new HtmlString(text.ToString());
        }
    }
}
