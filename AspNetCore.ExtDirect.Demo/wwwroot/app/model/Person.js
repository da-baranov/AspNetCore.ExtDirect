Ext.define("ExtDirectDemo.model.Person", {
    requires: ["Ext.data.identifier.Negative"],
    extend: "Ext.data.Model",
    idProperty: "id",
    identifier: "negative",
    fields: [
        { name: "id", type: "int" },
        "lastName",
        "firstName",
        "givenName"
    ],
    proxy: {
        type: "direct",
        api: {
            create: "Test.createPersons"
        }
    }
});