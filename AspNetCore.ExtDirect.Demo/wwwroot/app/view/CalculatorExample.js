Ext.define("ExtDirectDemo.view.CalculatorExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.CalculatorExample",
    layout: {
        type: "vbox",
        align: "stretch"
    },
    items: [
        {
            xtype: "numberfield",
            fieldLabel: "Operand 1",
            bind: {
                value: "{calculator.operand1}"
            }
        },
        {
            xtype: "numberfield",
            fieldLabel: "Operand 2",
            bind: {
                value: "{calculator.operand2}"
            }
        },
        {
            xtype: "button",
            text: "Calculate sum",
            itemId: "cmdCalculate1"
        },
        {
            xtype: "numberfield",
            fieldLabel: "Summa",
            bind: {
                value: "{calculator.sum}"
            }
        },
    ]
});