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

        public static IHtmlContent GetDateTimeInput(string id, string display, DateTime? value, bool onlyShow = false)
        {
            StringBuilder text = new StringBuilder();
            string valueHtml = value == null ? string.Empty : value.Value.ToString("yyyy-MM-ddTHH:mm");
            string disabledHtml = onlyShow ? "disabled" : string.Empty;
            string inputHiddenHtml = onlyShow ? $"<input name=\"{id}\"  type=\"text\" hidden value=\"{valueHtml}\">" : string.Empty;
            id = onlyShow ? "View"+id : id;
            text.Append($@"
                <label for=""{id}"">{display}:</label>
                <input name=""{id}"" data-validation=""filtrar"" class=""form-control"" type=""datetime-local"" {disabledHtml} value=""{valueHtml}"">
                {inputHiddenHtml}
            ");

            return new HtmlString(text.ToString());
        }

        public static IHtmlContent GetDateInput(string id, string display, DateTime? value, bool onlyShow = false)
        {
            StringBuilder text = new StringBuilder();
            string valueHtml = value == null ? string.Empty : value.Value.ToString("yyyy-MM-dd");
            string hiddenHtml = onlyShow ? "disabled" : string.Empty;
            string inputHiddenHtml = onlyShow ? $"<input name=\"{id}\" hidden type=\"text\" value=\"{valueHtml}\">" : string.Empty;
            id = onlyShow ? "View" + id : id;
            text.Append($@"
                <label for=""{id}"">{display}:</label>
                <input name=""{id}"" data-validation=""filtrar"" class=""form-control"" type=""date"" {hiddenHtml} value=""{valueHtml}"">
                {inputHiddenHtml}
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
