import requests
from odoo import models, fields, api

class InventoryImport(models.Model):
    _name = 'inventory.import'
    _description = 'Import Inventory Data from API'

    name = fields.Char(string="Name", default="Inventory Import")
    api_token = fields.Char(string="API Token", required=True)
    api_url = fields.Char(
        string="API URL",
        default="https://inventorymanagementapp-production-2db0.up.railway.app/api/inventory/"
    )

    aggregated_data = fields.Text(string="Aggregated Data", readonly=True)

    def action_import(self):
        for rec in self:
            url = rec.api_url + rec.api_token

            try:
                response = requests.get(url, timeout=10)
                if response.status_code != 200:
                    rec.aggregated_data = f"Error: {response.text}"
                    continue

                data = response.json()
                rec.aggregated_data = str(data.get("aggregated"))

            except Exception as e:
                rec.aggregated_data = f"Exception: {str(e)}"
