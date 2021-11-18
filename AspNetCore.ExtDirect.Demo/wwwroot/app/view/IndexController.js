Ext.define("ExtDirectDemo.view.IndexController", {
    extend: "Ext.app.ViewController",
    alias: "controller.IndexController",

    control: {
        "#": {
            beforerender: function () {
                const viewModel = this.getViewModel();

                const dataPollingProvider = Ext.direct.Manager.getProvider('DemoPolling');
                
                dataPollingProvider.on("data", function (provider, event) {
                    if (event.name === "ondata") {
                        viewModel.set("someRandomData", event.data);
                    }
                });

                const chatPollingProvider = Ext.direct.Manager.getProvider('ChatPolling');
                chatPollingProvider.on("data", function (provider, event) {
                    if (event.name === "onmessage") {
                        const store = viewModel.getStore("chat");
                        store.add(event.data);
                    }
                });
            }
        },

        "#cmdEvents": {
            click: function () {
                const viewModel = this.getViewModel();

                let pollingEnabled = viewModel.get("pollingEnabled");
                pollingEnabled = !pollingEnabled;
                viewModel.set("pollingEnabled", pollingEnabled);

                const pollingProvider = Ext.direct.Manager.getProvider('DemoPolling');

                if (pollingEnabled) {
                    pollingProvider.connect();
                } else {
                    pollingProvider.disconnect();
                }
            }
        },

        "#cmdHello": {
            click: function () {
                const name = this.getViewModel().get("name");
                Demo.hello(name, function (response, e) {
                    if (e.type === "exception") {
                        Ext.Msg.show(
                            {
                                title: "Error",
                                message: e.message,
                                icon: Ext.MessageBox.ERROR,
                                buttons: Ext.MessageBox.OK
                            }
                        );
                    } else {
                        Ext.Msg.alert('AspNetCore.ExtDirect', response);
                    }
                });
            }
        },

        "#cmdOrderedArguments": {
            click: function () {
                const args = this.getViewModel().get("orderedArguments");

                Demo.orderedArguments(args.a, args.b, args.c, function (response, e) {
                    if (e.type === "exception") {
                        Ext.Msg.show(
                            {
                                title: "Error",
                                message: e.message,
                                icon: Ext.MessageBox.ERROR,
                                buttons: Ext.MessageBox.OK
                            }
                        );
                    } else {
                        Ext.Msg.alert('AspNetCore.ExtDirect', 'Server said: ' + JSON.stringify(response));
                    }
                });
            }
        },

        "#cmdNamedArguments": {
            click: function () {
                const args = this.getViewModel().get("namedArguments");
                const data = {
                    a: args.a,
                    b: args.b,
                    c: args.c
                };
                Demo.namedArguments(data, function (response, e) {
                    if (e.type === "exception") {
                        Ext.Msg.show(
                            {
                                title: "Error",
                                message: e.message,
                                icon: Ext.MessageBox.ERROR,
                                buttons: Ext.MessageBox.OK
                            }
                        );
                    } else {
                        Ext.Msg.alert("AspNetCore.ExtDirect", "Server said: " + JSON.stringify(response));
                    }
                });
            }
        },

        "#cmdPersonName": {
            click: function () {
                const args = this.getViewModel().get("personName");
                const data = {
                    prefix: args.prefix,
                    firstName: args.firstName,
                    lastName: args.lastName
                };
                Demo.makeName(data, function (response, e) {
                    if (e.type === "exception") {
                        Ext.Msg.show(
                            {
                                title: "Error",
                                message: e.message,
                                icon: Ext.MessageBox.ERROR,
                                buttons: Ext.MessageBox.OK
                            }
                        );
                    } else {
                        Ext.Msg.alert('AspNetCore.ExtDirect', 'Server said that person full name is ' + response);
                    }
                });
            }
        },

        "#cmdSendMessage": {
            click: function () {
                const viewModel = this.getViewModel();
                const chatMessage = viewModel.get("chatMessage");
                Chat.sendMessage(chatMessage, function (response, e) {
                    if (e.type === "exception") {
                        Ext.Msg.show(
                            {
                                title: "Error",
                                message: e.message,
                                icon: Ext.MessageBox.ERROR,
                                buttons: Ext.MessageBox.OK
                            }
                        );
                    } else {
                        viewModel.set("chatMessage", "");
                    }
                });
            }
        }
    }
});