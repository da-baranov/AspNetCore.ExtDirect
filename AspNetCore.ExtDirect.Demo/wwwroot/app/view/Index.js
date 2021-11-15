Ext.define("ExtDirectDemo.view.Index", {
    requires: [
        "ExtDirectDemo.view.IndexController",
        "ExtDirectDemo.view.IndexViewModel"
    ],
    extend: "Ext.panel.Panel",
    controller: "IndexController",
    viewModel: "IndexViewModel",
    title: "AspNetCore.ExtDirect Demo",
    layout: {
        type: "vbox"
    },
    region: "center",
    bodyPadding: 10,
    items: [
        {
            xtype: "container",
            padding: 10,
            html: "<strong>Simple Hello World! example</strong>"
        },
        {
            xtype: "container",
            layout: "hbox",
            padding: 10,
            items: [
                {
                    xtype: "textfield",
                    reference: "txtName",
                    fieldLabel: "Your name",
                    bind: {
                        value: "{name}"
                    }
                },
                {
                    xtype: "button",
                    text: "Say hello",
                    itemId: "cmdHello"
                }
            ]
        },

        {
            xtype: "container",
            padding: 10,
            html: "<strong>Function with Ordered arguments call example</strong>"
        },
        {
            xtype: "container",
            layout: {
                type: "hbox",
                align: "end"
            },
            padding: 10,
            items: [
                {
                    xtype: "textfield",
                    reference: "txtO1",
                    fieldLabel: "Argument 1 (string)",
                    labelAlign: "top",
                    bind: {
                        value: "{orderedArguments.a}"
                    },
                    margin: 2
                },
                {
                    xtype: "numberfield",
                    reference: "txtO2",
                    fieldLabel: "Argument 2 (number)",
                    labelAlign: "top",
                    bind: {
                        value: "{orderedArguments.b}",
                    },
                    margin: 2
                },
                {
                    xtype: "datefield",
                    reference: "txtO3",
                    fieldLabel: "Argument 3 (date)",
                    labelAlign: "top",
                    bind: {
                        value: "{orderedArguments.c}"
                    },
                    margin: 2
                },
                {
                    xtype: "button",
                    text: "Call",
                    itemId: "cmdOrderedArguments",
                    margin: 2
                }
            ]
        },

        {
            xtype: "container",
            padding: 10,
            html: "<strong>Function with Named arguments call example</strong>"
        },
        {
            xtype: "container",
            layout: {
                type: "hbox",
                align: "end"
            },
            padding: 10,
            items: [
                {
                    xtype: "textfield",
                    reference: "txtN1",
                    fieldLabel: "Argument a (string)",
                    labelAlign: "top",
                    bind: {
                        value: "{namedArguments.a}"
                    },
                    value: "Some string",
                    margin: 2
                },
                {
                    xtype: "numberfield",
                    reference: "txtN2",
                    fieldLabel: "Argument b (double)",
                    labelAlign: "top",
                    bind: {
                        value: "{namedArguments.b}"
                    },
                    margin: 2
                },
                {
                    xtype: "numberfield",
                    reference: "txtN3",
                    fieldLabel: "Argument c (integer)",
                    labelAlign: "top",
                    decimalPrecision: 0,
                    bind: {
                        value: "{namedArguments.c}"
                    },
                    margin: 2
                },
                {
                    xtype: "button",
                    text: "Call",
                    itemId: "cmdNamedArguments",
                    margin: 2
                }
            ]
        },

        {
            xtype: "container",
            padding: 10,
            html: "<strong>Make person full name</strong>"
        },
        {
            xtype: "container",
            layout: {
                type: "hbox",
                align: "end"
            },
            padding: 10,
            items: [
                {
                    xtype: "combobox",
                    fieldLabel: "Prefix",
                    labelAlign: "top",
                    bind: {
                        value: "{personName.prefix}"
                    },
                    value: "Some string",
                    margin: 2,
                    store: ["Mr.", "Ms.", "Mrs."]
                },
                {
                    xtype: "textfield",
                    fieldLabel: "First name",
                    labelAlign: "top",
                    bind: {
                        value: "{personName.firstName}"
                    },
                    margin: 2
                },
                {
                    xtype: "textfield",
                    reference: "txtN3",
                    fieldLabel: "Last name",
                    labelAlign: "top",
                    decimalPrecision: 0,
                    bind: {
                        value: "{personName.lastName}"
                    },
                    margin: 2
                },
                {
                    xtype: "button",
                    text: "Call",
                    itemId: "cmdPersonName",
                    margin: 2
                }
            ]
        },

        {
            xtype: "container",
            padding: 10,
            html: "<strong>Ext Direct Polling (some random data from server) example</strong>"
        },
        {
            xtype: "container",
            layout: {
                type: "hbox",
                align: "center"
            },
            padding: 10,
            items: [                
                {
                    xtype: "button",
                    bind: {
                        text: "{pollingEnabled ? 'Stop events' : 'Start events'}"
                    },
                    itemId: "cmdEvents",
                    margin: 2
                },
                {
                    xtype: "label",
                    bind: {
                        text: "{someRandomData}"
                    },
                    margin: 2
                }
            ]
        },
    ]
});