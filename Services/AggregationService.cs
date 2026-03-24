using InventoryManagementApp.Model;
using InventoryManagementApp.Models;

namespace InventoryManagementApp.Services
{
    public class AggregationService
    {
        public object Build(Inventory inv)
        {
            var result = new List<object>();

            foreach (var field in inv.Fields)
            {
                var values = inv.Items
                    .Select(i => i.FieldValues.FirstOrDefault(v => v.FieldId == field.Id)?.Value)
                    .Where(v => v != null)
                    .ToList();

                if (!values.Any())
                {
                    result.Add(new
                    {
                        field.Title,
                        Type = field.Type.ToString(),
                        Empty = true
                    });
                    continue;
                }

                switch (field.Type)
                {
                    case FieldType.Number:
                        var nums = values.Select(double.Parse).ToList();
                        result.Add(new
                        {
                            field.Title,
                            Type = "number",
                            Min = nums.Min(),
                            Max = nums.Max(),
                            Avg = nums.Average()
                        });
                        break;

                    case FieldType.SingleLineText:
                    case FieldType.MultiLineText:
                        var top = values
                            .GroupBy(v => v)
                            .OrderByDescending(g => g.Count())
                            .Take(3)
                            .Select(g => g.Key)
                            .ToList();

                        result.Add(new
                        {
                            field.Title,
                            Type = "text",
                            Top = top
                        });
                        break;

                    case FieldType.Boolean:
                        var trues = values.Count(v => v == "true" || v == "True");
                        var falses = values.Count(v => v == "false" || v == "False");
                        var total = trues + falses;

                        result.Add(new
                        {
                            field.Title,
                            Type = "boolean",
                            PercentTrue = total == 0 ? 0 : trues * 100.0 / total,
                            PercentFalse = total == 0 ? 0 : falses * 100.0 / total
                        });
                        break;

                    case FieldType.DocumentLink:
                        result.Add(new
                        {
                            field.Title,
                            Type = "document",
                            Supported = false
                        });
                        break;
                }
            }

            return result;
        }
    }
}
