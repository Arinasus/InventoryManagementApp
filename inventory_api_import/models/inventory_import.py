import requests
from odoo import models, fields, api

class InventoryImport(models.Model):
    _name = 'inventory.import'
    _description = 'Import Inventory Data from API'

    name = fields.Char(default="Inventory Import")
    api_token = fields.Char(required=True)
    api_url = fields.Char(
        default="https://inventorymanagementapp-production-2db0.up.railway.app/api/inventory/"
    )

    aggregate_ids = fields.One2many(
        'inventory.aggregate',
        'import_id',
        string="Aggregated Data"
    )

    def action_import(self):
        for rec in self:
            url = rec.api_url + rec.api_token

            response = requests.get(url, timeout=10)
            if response.status_code != 200:
                raise ValueError("API error: " + response.text)

            data = response.json()
            aggregated = data.get("aggregated", [])

            rec.aggregate_ids.unlink()

            for item in aggregated:
                rec.env['inventory.aggregate'].create({
                    'import_id': rec.id,
                    'title': item.get('title'),
                    'type': item.get('type'),
                    'min_value': item.get('min'),
                    'max_value': item.get('max'),
                    'avg_value': item.get('avg'),
                    'top_values': ", ".join(item.get('top', [])) if item.get('top') else None,
                    'percent_true': item.get('percentTrue'),
                    'percent_false': item.get('percentFalse'),
                })
