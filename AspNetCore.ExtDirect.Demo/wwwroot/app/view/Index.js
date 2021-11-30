Ext.define("ExtDirectDemo.view.Index", {
    requires: [
        "ExtDirectDemo.view.IndexController",
        "ExtDirectDemo.view.IndexViewModel",
        "ExtDirectDemo.view.HelloExample",
        "ExtDirectDemo.view.CalculatorExample",
        "ExtDirectDemo.view.OrderedArgsExample",
        "ExtDirectDemo.view.NamedArgsExample",
        "ExtDirectDemo.view.MakePersonNameExample",
        "ExtDirectDemo.view.Chat",
        "ExtDirectDemo.view.RandomPollingDataExample",
        "ExtDirectDemo.view.RemotingStoreExample"
    ],
    extend: "Ext.tab.Panel",
    tabPosition: "left",
    tabRotation: 0,
    controller: "IndexController",
    viewModel: "IndexViewModel",
    plugins: "viewport",
    title: "ASPNETCORE.EXTDIRECT DEMO",
    region: "center",
    bodyPadding: 10,
    items: [
        {
            xtype: "HelloExample"
        },
        {
            xtype: "CalculatorExample"
        },
        {
            xtype: "OrderedArgsExample"
        },
        {
            xtype: "NamedArgsExample"
        },
        {
            xtype: "MakePersonNameExample"
        },
        {
            xtype: "RandomPollingDataExample"
        },
        {
            xtype: "Chat"
        },
        {
            xtype: "RemotingStoreExample"
        }
    ]
});