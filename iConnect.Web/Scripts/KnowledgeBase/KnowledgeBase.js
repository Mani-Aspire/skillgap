$(function () {
    $("#divtree")
        .jstree({
            "plugins": ["themes", "html_data", "ui", "crrm", "contextmenu"],
            "contextmenu": {
                "items": function (obj) {
                    var config = {};

                    config["create"] = {
                        "label": "Create",
                        "action": function (obj) { this.create(obj); }
                    };

                    config["Attach"] = {
                        "label": "Attach",
                        "action": function (obj) { return AttachFile(obj); }
                    };

                    if ($.jstree._focused()._get_parent(obj) != -1) {
                        config["remove"] = {
                            "label": "Remove",
                            "action": function (obj)
                            { this.remove(obj); }
                        };
                    }
                    return config;
                }
            }
        }).bind("create.jstree", function (e, data) {
            if (FileNameIsValid(data.rslt.name) == false) {
                alert("Invalid file name");
                $.jstree.rollback(data.rlbk);
                return;
            }

            var fileDate = JSON.stringify({ selectedPath: data.rslt.obj.parents("li").attr('path'), folderName: data.rslt.name });
            $.ajax({
                type: "POST",
                url: "/KnowledgeBase/AddDirectory",
                data: fileDate,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successFunc,
                error: errorFunc
            });
            function successFunc(data, status) {
                alert(data);
            }
            function errorFunc(response) {
                alert(response.status + ' ' + response.statusText);
                return;
            }
            $(data.rslt.obj).attr("path", data.rslt.obj.parents("li").attr('path') + "\\" + data.rslt.name);
        }).bind("remove.jstree", function (e, data) {
            //  alert(data.rslt.obj.attr('path'));
            var fileDate = JSON.stringify({ folderPath: data.rslt.obj.attr('path') });
            $.ajax({
                type: "POST",
                url: "/KnowledgeBase/DeleteDirectory",
                data: fileDate,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successFunc,
                error: errorFunc
            });
            function successFunc(data, status) {
                alert(data);
            }
            function errorFunc(response) {
                alert(response.status + ' ' + response.statusText);
                return;
            }
        }).bind("before.jstree", function (e, data) {
            if (data.func === "remove") {
                if (!confirm('Are you sure you want to delete this folder?')) {
                    e.stopImmediatePropagation();
                    return false;
                }
            }
        }).bind("select_node.jstree", function (e, data) {
            //data.inst.open_node(data.rslt.obj); //used to close inactive branches 
            displayBreadCrumb(e, data);
        });
});


    function displayBreadCrumb(e, data) {
    var pathString = "";
    var nodeString = "";
    data.rslt.obj.parents("li").each(function () {

        nodeString = ' <a onclick="SelectNodeByPath(\'' + $(this).attr('path').replace(/a|\\/g, "\\$&") + '\')">' + $(this).children("a").text() + '</a>';
        pathString = nodeString + " <img src='../../CSS/Images/rightArrow.jpeg' width='10px' height='10px'/> " + pathString; // <-- this not working
    });
    pathString = "Knowledge Base <img src='../../CSS/Images/rightArrow.jpeg' width='10px' height='10px'/> " + pathString + ' <a onclick="SelectNodeByPath(\'' + data.rslt.obj.attr('path').replace(/a|\\/g, "\\$&") + '\')">' + data.rslt.obj.children('a').text() + '</a>';
    $('#knowledgebrdcrb').html(pathString);
}

function SelectNodeByPath(nodePath) {
    var $tree = $("#divtree"),
    node = $tree.find('li').filter(function () {
        return $(this).attr('path') === nodePath;
    });
    $tree.jstree('deselect_all');
    $tree.jstree('select_node', node);
}

function AttachFile(obj) {
    $('#AttachFileDialog').modal('show');
    $('#hdnFilePath').val(obj[0].attributes['path'].value);
    $("#file1").val('');
}

function FileNameIsValid(elem) {
    var regex = new RegExp("^[ _0-9a-zA-Z\$\%\'\-\@\{\}\~\!\#\(\)\&\^]");
    return regex.test(elem);
}

function Cancel() {
    $('#AttachFileDialog').dialog('close');
}

function uploadFile() {
    var fileInput = document.getElementById('file1');
    var file = $('input[type=file]').val();
    if (!file) {
        alert('Please attach file');
        return;
    }
    else {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/KnowledgeBase/UploadTest', false);
        xhr.setRequestHeader('Content-type', 'multipart/form-data');
        //Appending file information in Http headers
        xhr.setRequestHeader('X-File-Name', fileInput.files[0].name);
        xhr.setRequestHeader('X-File-Type', fileInput.files[0].type);
        xhr.setRequestHeader('X-File-Size', fileInput.files[0].size);
        xhr.setRequestHeader('X-Root-Path', $('#hdnFilePath').val());
        //Sending file in XMLHttpRequest
        xhr.send(fileInput.files[0]);
        result = xhr.response;

        if (result) {
            alert('File has been uploaded successfully!');
            Cancel();
        }
    }
    return result;
}

function BindTree() {
    alert('hi');
    $.ajax({
        type: "POST",
        url: "/Home/Index1",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunc,
        error: errorFunc
    });
    function successFunc(data, status) {
        var viewModel = data;
        alert(viewModel);
        ko.applyBindings(viewModel);
    }
    function errorFunc(response) {
        alert(response.status + ' ' + response.statusText);
        return;
    }
}

var Node = function (name, NodePath, ParentNodePath, Folders) {
    this.name = ko.observable(name);
    this.NodePath = ko.observable(NodePath);
    this.ParentNodePath = ko.observable(ParentNodePath);
    this.Folders = ko.observableArray(Folders);
};

//BindTree();


