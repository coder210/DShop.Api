using Scriban;
using Scriban.Runtime;
using System.Text.RegularExpressions;

namespace DShop.PluginShared
{
    /// <summary>
    /// HTML 模板引擎，基于 Scriban（高性能 .NET 模板引擎）。
    /// 支持：
    /// - {{key}} 单值变量替换
    /// - {{#each CollectionName}}...{{/each}} 循环
    /// - {{#if VarName}}...{{else}}...{{/if}} 条件判断
    /// - {{ for item in CollectionName }}...{{ end }} 集合循环（键不区分大小写）
    /// 本类为纯静态、无状态工具类，可被多个插件共享使用（各插件 ALC 独立加载副本，互不影响）。
    /// </summary>
    public static class HtmlTemplateEngine
    {
        /// <summary>
        /// 替换 HTML 模板中的占位符（支持 {{key}} 格式和 {{#if}} 条件）
        /// </summary>
        public static string ReplacePlaceholders(string htmlTemplate, Dictionary<string, string> data)
        {
            return ReplacePlaceholders(htmlTemplate, data, null);
        }

        /// <summary>
        /// 替换 HTML 模板中的占位符，支持集合循环数据。
        /// 集合以 {{ for item in CollectionName }}...{{ end }} 语法在模板中遍历，
        /// 项字段通过 item.FieldName 访问（键不区分大小写）。
        /// </summary>
        public static string ReplacePlaceholders(
            string htmlTemplate,
            Dictionary<string, string> data,
            Dictionary<string, List<Dictionary<string, object>>>? collections)
        {
            if (string.IsNullOrEmpty(htmlTemplate)) return htmlTemplate;

            htmlTemplate = ProcessConditionals(htmlTemplate, data);
            return RenderWithScriban(htmlTemplate, data, collections);
        }

        /// <summary>
        /// 处理带循环的模板（完全兼容原有 Handlebars 风格语法）
        /// </summary>
        public static string ProcessTemplate(
            string htmlTemplate,
            Dictionary<string, string> singleValues,
            Dictionary<string, List<Dictionary<string, string>>> collections)
        {
            if (string.IsNullOrEmpty(htmlTemplate)) return htmlTemplate;

            var result = htmlTemplate;

            // 1. 先处理顶级 {{#if}} 条件块（可能包裹着 {{#each}}）
            result = ProcessConditionals(result, singleValues);

            // 2. 处理 {{#each CollectionName}}...{{/each}} 循环块
            var eachRegex = new Regex(@"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}", RegexOptions.Singleline);
            result = eachRegex.Replace(result, match =>
            {
                var collectionName = match.Groups[1].Value;
                var blockContent = match.Groups[2].Value;

                if (!collections.TryGetValue(collectionName, out var items) || items == null || items.Count == 0)
                    return string.Empty;

                var rows = new List<string>();
                foreach (var item in items)
                {
                    var row = RenderWithScriban(blockContent, item);
                    rows.Add(row);
                }

                return string.Join("", rows);
            });

            // 3. 替换剩余的 {{key}} 单值变量
            result = RenderWithScriban(result, singleValues);

            return result;
        }

        /// <summary>
        /// 使用 Scriban 渲染模板，可选注入集合数据（{{ for item in Collection }} 循环）
        /// </summary>
        private static string RenderWithScriban(
            string templateText,
            Dictionary<string, string> data,
            Dictionary<string, List<Dictionary<string, object>>>? collections = null)
        {
            if (string.IsNullOrEmpty(templateText)) return templateText;

            var template = Template.Parse(templateText);
            if (template.HasErrors)
                return templateText;

            var scriptObject = new ScriptObject();
            foreach (var kvp in data)
            {
                scriptObject[kvp.Key] = kvp.Value;
            }

            if (collections != null)
            {
                foreach (var collection in collections)
                {
                    var array = new ScriptArray();
                    foreach (var item in collection.Value)
                    {
                        var itemObj = new ScriptObject();
                        foreach (var kv in item)
                        {
                            itemObj[kv.Key] = kv.Value;
                        }
                        array.Add(itemObj);
                    }
                    scriptObject[collection.Key] = array;
                }
            }

            var context = new TemplateContext { StrictVariables = false };
            context.PushGlobal(scriptObject);
            return template.Render(context);
        }

        /// <summary>
        /// 处理 {{#if VarName}}...{{else}}...{{/if}} 条件块
        /// </summary>
        private static string ProcessConditionals(string template, Dictionary<string, string> data)
        {
            var ifRegex = new Regex(@"\{\{#if\s+(\w+)\}\}(.*?)(?:\{\{else\}\}(.*?))?\{\{/if\}\}", RegexOptions.Singleline);
            return ifRegex.Replace(template, match =>
            {
                var varName = match.Groups[1].Value;
                var ifContent = match.Groups[2].Value;
                var elseContent = match.Groups[3].Success ? match.Groups[3].Value : "";

                var hasValue = data.TryGetValue(varName, out var val);
                var isTruthy = hasValue && !string.IsNullOrEmpty(val)
                    && val.Trim().ToLower() != "false"
                    && val.Trim() != "0"
                    && val.Trim().ToLower() != "no";

                var branch = isTruthy ? ifContent : elseContent;
                return RenderWithScriban(branch, data);
            });
        }
    }
}
