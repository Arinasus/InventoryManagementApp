from odoo import models, fields

class InventoryAggregate(models.Model):
    _name = 'inventory.aggregate'
    _description = 'Aggregated Inventory Data'

    import_id = fields.Many2one('inventory.import', string="Import")
    title = fields.Char(string="Field Title")
    type = fields.Char(string="Field Type")
    min_value = fields.Float(string="Min")
    max_value = fields.Float(string="Max")
    avg_value = fields.Float(string="Average")
    top_values = fields.Char(string="Top Values")
    percent_true = fields.Float(string="True %")
    percent_false = fields.Float(string="False %")
