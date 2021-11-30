Ext.define("ExtDirectDemo.view.RandomPollingDataExample", {
    extend: "Ext.panel.Panel",
    title: "Random polling data example",
    alias: "widget.RandomPollingDataExample",
    layout: {
        type: "hbox",
        align: "start"
    },
    padding: 10,
    items: [
        {
            xtype: "button",
            bind: {
                text: "{pollingEnabled ? 'Stop polling' : 'Start polling'}"
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
});