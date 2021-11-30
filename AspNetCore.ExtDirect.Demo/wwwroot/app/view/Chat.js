Ext.define("ExtDirectDemo.view.Chat", {
    extend: "Ext.panel.Panel",
    title: "Chat",
    alias: "widget.Chat",
    layout: {
        type: "border",
    },
    items: [
        {
            xtype: "container",
            region: "north",
            layout: {
                type: "hbox",
                align: "end"
            },
            items: [
                {
                    xtype: "textfield",
                    fieldLabel: "Say something",
                    bind: {
                        value: "{chatMessage}"
                    }
                },
                {
                    xtype: "button",
                    text: "Send",
                    itemId: "cmdSendMessage"
                }
            ]
        },
        {
            xtype: "gridpanel",
            region: "center",
            columns: [
                {
                    text: "Date/time",
                    flex: 1,
                    dataIndex: "date",
                    format: "c"
                },
                {
                    text: "You said",
                    flex: 3,
                    dataIndex: "message"
                }
            ],
            bind: {
                store: "{chat}"
            }
        }
    ]
});