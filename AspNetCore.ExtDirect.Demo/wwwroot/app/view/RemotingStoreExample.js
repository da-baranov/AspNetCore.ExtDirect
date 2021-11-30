Ext.define("ExtDirectDemo.view.RemotingStoreExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.RemotingStoreExample",
    title: "Remoting store example",
    layout: {
        type: "border"
    },
    items: [
        {
            xtype: "grid",
            region: "center",
            title: "Remoting store example",
            tbar: [
                {
                    xtype: "button",
                    text: "Add...",
                    itemId: "cmdPersonsAdd"
                },
                {
                    xtype: "button",
                    text: "Delete"
                },
                {
                    xtype: "button",
                    text: "Refresh",
                    itemId: "cmdPersonsRefresh"
                },
                '-',
                {
                    xtype: "label",
                    text: "Search: "
                },
                {
                    xtype: "textfield",
                    itemId: "txtPersonSearch",
                    bind: {
                        value: "{personsView.filter}"
                    }
                }
            ],
            columns: [
                {
                    text: "Last Name",
                    dataIndex: "lastName",
                    flex: 1
                },
                {
                    text: "First Name",
                    dataIndex: "firstName",
                    flex: 1
                },
                {
                    text: "Given Name",
                    dataIndex: "givenName",
                    flex: 1
                }
            ],
            bind: {
                store: "{persons}"
            }
        }
    ]
});