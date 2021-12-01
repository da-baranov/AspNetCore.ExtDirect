Ext.define("ExtDirectDemo.view.RandomPollingDataExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.RandomPollingDataExample",
    layout: {
        type: "hbox",
        align: "start"
    },
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