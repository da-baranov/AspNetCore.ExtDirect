Ext.define("ExtDirectDemo.view.RemotingStoreExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.RemotingStoreExample",
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
                    iconCls: "fa fa-plus",
                    itemId: "cmdPersonsAdd"
                },
                {
                    xtype: "button",
                    text: "Delete",
                    iconCls: "fa fa-minus",
                    itemId: "cmdPersonsDelete"
                },
                {
                    xtype: "button",
                    text: "Refresh",
                    itemId: "cmdPersonsRefresh",
                    iconCls: "fa fa-sync"
                },
                '-',
                {
                    xtype: "label",
                    text: "Search: "
                },
                {
                    xtype: "textfield",
                    itemId: "txtPersonSearch",
                    iconCls: "fa fa-search",
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
                store: "{persons}",
                selection: "{personsView.selection}"
            }
        }
    ]
});