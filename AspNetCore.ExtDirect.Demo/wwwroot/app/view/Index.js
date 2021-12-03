Ext.define("ExtDirectDemo.view.Index", {
    requires: [
        "ExtDirectDemo.view.IndexController",
        "ExtDirectDemo.view.IndexViewModel",
        "ExtDirectDemo.view.HelloExample",
        "ExtDirectDemo.view.CalculatorExample",
        "ExtDirectDemo.view.OrderedArgsExample",
        "ExtDirectDemo.view.NamedArgsExample",
        "ExtDirectDemo.view.MakePersonNameExample",
        "ExtDirectDemo.view.FormExample",
        "ExtDirectDemo.view.Chat",
        "ExtDirectDemo.view.RandomPollingDataExample",
        "ExtDirectDemo.view.RemotingStoreExample"
    ],
    extend: "Ext.panel.Panel",
    // tabPosition: "left",
    // tabRotation: 0,
    controller: "IndexController",
    viewModel: "IndexViewModel",
    plugins: "viewport",
    title: "ASPNETCORE.EXTDIRECT Examples",
    region: "center",
    layout: {
        type: "accordion"
    },
    defaults: {
        bodyStyle: 'padding:15px'
    },
    items: [
        {
            xtype: "HelloExample",
            title: "<b>1. Hello example</b>"
        },
        {
            xtype: "CalculatorExample",
            title: "<b>2. Calculator service example</b>"
        },
        {
            xtype: "OrderedArgsExample",
            title: "<b>3. Calling a remote function with ordered arguments</b>",
        },
        {
            xtype: "NamedArgsExample",
            title: "<b>4. Calling a remote function with named arguments</b>",
        },
        {
            xtype: "MakePersonNameExample",
            title: "<b>5. Make Person name</b>",
        },
        {
            xtype: "RandomPollingDataExample",
            title: "<b>6. Random polling data example</b>",
        },
        {
            xtype: "FormExample",
            title: "<b>7. Form post/upload example</b>",
        },
        {
            xtype: "Chat",
            title: "<b>8. Chat demo</b>"
        },
        {
            xtype: "RemotingStoreExample",
            title: "<b>9. Working with a store that uses Ext Direct</b>",
        }
    ]
});