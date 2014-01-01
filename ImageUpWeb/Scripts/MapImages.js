
var uri = 'api/MapImages';
var mapUrl = 'http://www.bthedifference.org/';

$(document).ready(function () {
    loadMapImages();

    $("#btnUploadFile").click(OnUpload);
    $("#btnDiscard").click(DiscardFile);

    $('#dvImageMapDetail').popup({
        opacity: 0.6,
        transition: 'all 0.3s'
    });
});

function loadMapImages() {
    // Send an AJAX request
    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of images.
            $('#tblBMapImages tr td').unbind('click');
            $('#tblBMapImages').empty();
            $.each(data, function (key, item) {
                $('#tblMapImages').append(
                    $(document.createElement('tr')).append(
                        $(document.createElement('td')).addClass('editLink').append($(document.createElement('span')).html('edit').bind('click', function () { EditMapImage(item.MapImageId); }))
                    ).append(
                        $(document.createElement('td')).html(item.Lat)
                    ).append(
                        $(document.createElement('td')).html(item.Long)
                    ).append(
                        $(document.createElement('td')).html(item.Desc)
                    ).append(
                        $(document.createElement('td')).html(item.SentBy)
                    ).append(
                        $(document.createElement('td')).html(item.Comment)
                    ).append(
                        $(document.createElement('td')).html(item.ImageUrl)
                    ).append(
                        $(document.createElement('td')).html(item.VideoURL)
                    )

                    /*.append(
                        $(document.createElement('td')).html('delete').bind('click', function () { DeleteMapImage(item.MapImageId); })
                    ).append(
                        $(document.createElement('td')).html()
                    )*/
                );
            });
            $('#tblMapImages').tableScroll({ height: 600});
        });
}


function formatItem(item) {
    return item.Desc + ': $' + item.Comment;
}

function EditMapImage(id) {
    $('#imgMapImage').attr('src', '');
    $('#iVideo').attr('src', '');
    var uri = 'api/MapImages/{0}';
    uri = uri.replace('{0}', id);
    $.ajax({
        type: "GET",
        url: uri,
        contentType: false,
        processData: false,
        success: function (results) {
            for (i = 0; i < results.length; i++) {
                $('#txtLat').val(results[i].Lat);
                $('#txtLong').val(results[i].Long);
                $('#txtDesc').val(results[i].Desc);
                $('#txtSentBy').val(results[i].SentBy);
                $('#txtComment').val(results[i].Comment);
                $('#txtImageUrl').val(results[i].ImageUrl);
                $('#txtVideoUrl').val(results[i].VideoURL);
                if (results[i].ImageUrl != null || results[i].ImageUrl == '') {
                    $('#imgMapImage').attr('src', mapUrl + results[i].ImageUrl);
                    $('#imgMapImage').show();
                    $('#iVideo').hide();
                } else {
                    $('#iVideo').attr('src', results[i].VideoURL);
                    $('#iVideo').show();
                    $('#imgMapImage').hide();
                }
                $('#btnMapImageSave').data('MapImageId', results[i].MapImageId);
            }
            //Show edit
            $('#dvImageMapDetail').popup('show');
        }
    });
}

function UpdateMapImage(evt) {

    var MapImageId = $(evt).data('MapImageId');
    var json = JSON.stringify(createMapImageJSON(MapImageId));
    blockUI();

    $.ajax({
        url: '/api/MapImages/' + MapImageId,
        type: 'PUT',
        contentType: "application/json; charset=utf-8",
        data: json,
        success: function (results) {
            $.unblockUI();
            //hide detail
            $('#dvImageMapDetail').popup('hide');
            loadMapImages();
        },
        error: function (error) {
            $.unblockUI();
            alert(error);
        }
    });
    $('#imgMapImage').attr('src', '');
    $('#iVideo').attr('src', '');
}

function DeleteMapImage(id) {

}

function createMapImageJSON(id) {
    jsonObj = [];

    item = {}
    item["MapImageId"] = id;
    item["Lat"] = $('#txtLat').val();
    item["Long"] = $('#txtLong').val();
    item["Desc"] = $('#txtDesc').val();
    item["SentBy"] = $('#txtSentBy').val();
    item["Comment"] = $('#txtComment').val();
    item["ImageUrl"] = $('#txtImageUrl').val();
    item["VideoURL"] = $('#txtVideoUrl').val();

    jsonObj.push(item);


    return item;
}

function find() {
    var id = $('#prodId').val();
    $.getJSON(uri + '/' + id)
        .done(function (data) {
            $('#product').text(formatItem(data));
        })
        .fail(function (jqXHR, textStatus, err) {
            $('#product').text('Error: ' + err);
        });
}

function blockUI(){
    $.blockUI({
        css: {
        border: 'none',
        padding: '15px',
        backgroundColor: '#000',
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        opacity: .5,
        color: '#fff'
    }
    });
}

function OnUpload(evt) {
    var files = $("#flMapImage").get(0).files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("file" + i, files[i]);
            }
            blockUI();

            $.ajax({
                type: "POST",
                url: "/api/Uploader",
                contentType: false,
                processData: false,
                data: data,
                success: function (results) {
                    //alert("Successfully uploaded " + results.length + " file(s).");
                    loadMapImages();
                    $.unblockUI();
                    //Open edit window
                    EditMapImage(results[0].MapImageId);
                }
            });
        } else {
            $.unblockUI();
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
}

    function DiscardFile() {
        var fileName = $('#spnFileName').html();

        var apiUrl = "api/uploader/{0}";
        apiUrl = apiUrl.replace("{0}", fileName);


        $.ajax({
            url: apiUrl,
            type: 'DELETE',
            cache: false,
            statusCode: {
                200: function (data) {
                }, // Successful DELETE
                404: function (data) {
                    alert(apiUrl + " ... Not Found");
                }, // 404 Not Found
                400: function (data) {
                    alert("Bad Request O");
                } // 400 Bad Request
            } // statusCode
        }); // ajax call

    }