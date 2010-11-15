function setAutoProperty(propertyName, nodeId, nodeTypeAlias, useParent)
{
    jQuery.get('/umbraco/webservices/autoproperty/autopropertyservice.asmx/getpropertyitems'
        , { 'PropertyName': propertyName, 'NodeId': nodeId, 'NodeTypeAlias': nodeTypeAlias, 'UseParent': useParent }
        , function (data) {
            var arr = []; //initiera en tom array. Samma sak som new Array();
            $(data).find("string").each(function () {
                var node = $(this);
                arr.push(node.text()); //lägg till ny post i arrayen
            });
            jQuery('.autoPropertyTextField.' + propertyName).autocomplete(arr);
        });
}

function includeCSS(p_file) {
    var v_css = document.createElement('link');
    v_css.rel = 'stylesheet'
    v_css.type = 'text/css';
    v_css.href = p_file;
    document.getElementsByTagName('head')[0].appendChild(v_css);
}