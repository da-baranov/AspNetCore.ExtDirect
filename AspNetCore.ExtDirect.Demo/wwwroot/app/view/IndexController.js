Ext.define("ExtDirectDemo.view.IndexController", {
    extend: "Ext.app.ViewController",
    requires: "ExtDirectDemo.model.Person",
    alias: "controller.IndexController",

    control: {
        "#": {
            beforerender: function () {
                const viewModel = this.getViewModel();

                const dataPollingProvider = Ext.direct.Manager.getProvider("POLLING_DATA_API");

                if (dataPollingProvider) {
                    dataPollingProvider.on("data", function (provider, event) {
                        if (event.name === "ondata") {
                            viewModel.set("someRandomData", event.data);
                        }
                    });
                }

                const chatPollingProvider = Ext.direct.Manager.getProvider("POLLING_CHAT_API");

                if (chatPollingProvider != null) {
                    chatPollingProvider.on("data", function (provider, event) {
                        if (event.name === "onmessage") {
                            const store = viewModel.getStore("chat");
                            store.add(event.data);
                        }
                    });
                }
            }
        },

        "#cmdEvents": {
            click: function () {
                const viewModel = this.getViewModel();

                let pollingEnabled = viewModel.get("pollingEnabled");
                pollingEnabled = !pollingEnabled;
                viewModel.set("pollingEnabled", pollingEnabled);

                const pollingProvider = Ext.direct.Manager.getProvider('POLLING_DATA_API');

                if (pollingEnabled) {
                    pollingProvider.connect();
                } else {
                    pollingProvider.disconnect();
                }
            }
        },

        "#cmdCalculate1": {
            click: function () {
                const model = this.getViewModel();
                const op1 = model.get("calculator.operand1");
                const op2 = model.get("calculator.operand2");
                Calculator1.Calculator.add(op1, op2, function (response, e) {
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
                        model.set("calculator.sum", response);
                    }
                });
            }
        },

        "#cmdCalculate2": {
            click: function () {
                Calculator2.Calculator.add(2, 2, function (response, e) {
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

        "#cmdHello": {
            click: function () {
                const name = this.getViewModel().get("name");
                Test.hello(name, function (response, e) {
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

                Test.orderedArguments(args.a, args.b, args.c, function (response, e) {
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
                Test.namedArguments(data, function (response, e) {
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
                Test.makeName(data, function (response, e) {
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
        },

        "#cmdPersonsAdd": {
            click: function () {
                const store = this.getStore("persons");
                const window = Ext.create("ExtDirectDemo.view.PersonForm");
                window.on("ok", function (sender, data) {
                    const model = Ext.create("ExtDirectDemo.model.Person", data);
                    model.save({
                        success: function () {
                            window.close();
                            // store.reload();
                        },
                        failure: function (record, operation) {
                            const errorMessage = operation.error;
                            Ext.Msg.show(
                                {
                                    title: "Error",
                                    message: errorMessage,
                                    icon: Ext.MessageBox.ERROR,
                                    buttons: Ext.MessageBox.OK
                                }
                            );
                        }
                    });
                });
                window.show();
            }
        },

        "#cmdPersonsRefresh": {
            click: function () {
                const store = this.getStore("persons");
                store.reload();
            }
        },

        "#cmdPersonsDelete": {
            click: function () {
                var selection = this.getViewModel().get("personsView.selection");
                if (selection) {
                    const store = this.getStore("persons");
                    store.remove(selection);
                    store.sync();
                }
            }
        },

        "#txtPersonSearch": {
            change: function (sender) {
                const store = this.getStore("persons");
                const filter = sender.getValue();
                store.getProxy().setExtraParam("filter", filter);
                store.reload();
            }
        }
    }
});