
        $(document).ready(function () {
            $("#ddlSlabColorID").change(function () {
                $.ajax({
                    url: ROOT + "/OnlineQuote/GetSlabImage",
                    type: "GET",
                data: { SlabColorID: $('#ddlSlabColorID').val() }
            })
                .done(function (data) {
                    if (data != '') {
                        $("#imgSlab").addClass("hidden");
                        $("#vidMeasure").hide();
                        $("#ajaxLoader").show()
                        $("#imgSlab").attr("src", "http://granitesouthlake.com/images/Slabs/" + data);
                        $("#imgSlab").load(function () {
                            $("#ajaxLoader").hide();
                            $("#vidMeasure").hide();
                            $("#imgSlab").removeClass("hidden");
                        });
                    }
                    else {
                        $("#imgSlab").addClass("hidden");
                        $("#vidMeasure").removeClass("hidden");
                        $("#vidMeasure").show();
                    }
                });
            ValidateSlabInfo(this.id);
        });

$('#SFQuantity').bind('blur', function (e) {
    if ($(this).val() > 0) {
        $(this).closest('.form-group').find('.help-block').addClass('hidden');                    
    }
    else {
        $(this).closest('.form-group').find('.help-block').removeClass('hidden');
    }
    EnableCustomerButton();
});

$('#SFQuantity').focus(function () {
    $("#imgSlab").addClass("hidden");
    $("#SfMsg").removeClass("hidden");
    $("#vidMeasure").removeClass("hidden");
    $("#vidMeasure").show();
}).blur(function () {
    $("SfMsg").addClass("hidden");
    if (!$("#vidMeasure").is(":hover") && $('#ddlSlabColorID').val() != "") {
        $("#ajaxLoader").hide();
        $("#vidMeasure").hide();
        $("#imgSlab").removeClass("hidden");
    }
});

$("#btnSendTableData").on("click", function () {
    $.ajax({
        url: ROOT + "/SlabPromo",
    type: "GET",
    data: { limit: 0 }
})
    .done(function (partialViewResult) {
        $("#_SlabPromo").html(partialViewResult);
        $("#ViewAllSlab").hide();
    });
});

function tblMeasureValues() {
    var TableData = new Array();

    $('#tblMeasures>tbody tr').each(function (row, tr) {
        TableData[row] = {
            "measure": $(tr).find("#measure").val()
            , "width": $(tr).find("#width").val()
            , "length": $(tr).find("#length").val()
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}

function tblSinkValues() {
    var TableData = new Array();

    $('#tblSinks>tbody tr').each(function (row, tr) {
        TableData[row] = {
            "sinkid": $(tr).find('select').val()
            , "quantity": $(tr).find('input').val()
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}

function tblServiceValues() {
    var TableData = new Array();

    $('#tblServices>tbody tr.selectedRow').each(function (row, tr) {
        TableData[row] = {
            "servicesid": $(tr).find('input[type="checkbox"]').attr('id')
        }
    });

    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

$("#submit-quote").click(function () {
    $('#cancel-quote').prop('disabled', true);

    var l = Ladda.create(this);
    l.start();

    var tableMeasures, tableSinks, tableServices, customerInfo;
    tableMeasures = $.toJSON(tblMeasureValues());
    tableSinks = $.toJSON(tblSinkValues());
    tableServices = $.toJSON(tblServiceValues());
    customerInfo = JSON.stringify($('form').serializeObject())
    //alert('JSON array to send to server: \n\n' + TableData.replace(/},/g, "},\n"));
    //alert(TableData);

    $.ajax({
        url: ROOT + "/OnlineQuote/AddOnlineQuote",
        type: 'POST',
        dataType: 'text',
        data: "jsonMeasures=" + tableMeasures
            + "&SlabColorID=" + encodeURIComponent($('#ddlSlabColorID').val())
            + "&SFQty=" + encodeURIComponent($('#SFQuantity').val())
            + "&EdgeID=" + encodeURIComponent($('#ddlEdges').val())
            + "&jsonSinks=" + tableSinks
            + "&jsonServices=" + tableServices
            + "&jsonCustomer=" + customerInfo
            + "&Notes=" + encodeURIComponent($('#notes').val())
            + "&SlabPromoOveride=" + encodeURIComponent($('#SlabPromoOveride').val())

    })
    .done(function (data) {
        if (data == 'err') {
            alert('Oops! We encountered a problem creating your quote. Please try again.');
            $('#cancel-quote').prop('disabled', false);
        }
        else {
            $('#modalCustomer').modal('hide');
            alert("Quote successfully created.")
            window.location = data;
        }
    })
    .fail(function (xhr, ajaxOptions, thrownError) {
        console && console.log("request failed");
    });


});

$("#ddlEdges").change(function () {
    ValidateSlabInfo(this.id);
    if (this.value == -1)
        $('#modalEdges').modal({ "show": true });                
});

//$(".ddlSink").change(function () {
//    if (this.value == -1)
//        $('#modalSinks').modal({ "show": true });
//});

$("#modalEdges a.imgEdge").click(function () {                
    $("#ddlEdges").val(this.dataset['edgeid']);
    ValidateSlabInfo('ddlEdges');
});

//Tick the Service Checkbox when row is click
$('#tblServices tr').click(function (event) {
    if (event.target.type !== 'checkbox') {
        $(':checkbox', this).trigger('click');
    }
});

$("#tblServices input[type='checkbox']").change(function (e) {
    if ($(this).is(":checked")) {
        $(this).closest('tr').addClass("selectedRow");
    } else {
        $(this).closest('tr').removeClass("selectedRow");
    }
});

$("#divShowCustomerModal").mouseenter(function () {
    EnableCustomerButton();
});

$("#div-submit-quote").mouseenter(function () {
    if ($('#verify_contact').val().trim() == '' | $('#Email').val().trim() == '' | $('#CustomerFirstName').val().trim() == '' | $('#CustomerLastName').val().trim() == '') {
        $('#submit-quote').prop('disabled', true);
    }
});

});

function showSinkModal(show, id) {
    if (show == -1) {
        $('#modalSinks').modal({ "show": true });
        $('#modalSinks a.imgSink').attr('data-setsinkid', id);
        $('.close-sinkmodal').attr('data-setsinkid', id);
    }
    else {
        $('#' + id).siblings("span").text($('#' + id + " option[value='" + $('#' + id).val() + "']").text());
    }

    $("#modalSinks a.imgSink").click(function () {
        var ddlSinkID = '#' + this.dataset['setsinkid'];
        var selectedSink = this.dataset['sinkid']
        $(ddlSinkID).val(selectedSink);
        $(ddlSinkID).siblings("span").text($(ddlSinkID + " option[value='" + selectedSink + "']").text());
    });
}

$(".close-sinkmodal").click(function () {
    var ddlSinkID = '#' + this.dataset['setsinkid'];
    $(ddlSinkID).val(0);
    $(ddlSinkID).siblings("span").text('');
});

$(document).on('change', '#showSinkName', function () {
    var checked = $(this).is(":checked");

    //var index = $(this).parent().index();
    $('#tblSinks tbody tr').each(function () {
        if (checked) {
            $(this).find("td").eq(2).hide();
            $('.ddlSink').hide();;
            $('.ddlSink').siblings("span").removeClass("hidden");
        } else {
            $(this).find("td").eq(2).show();
            $('.ddlSink').show();
            $('.ddlSink').siblings("span").addClass("hidden");
        }
    });
});

function ValidateSlabInfo(id) {
    if ($('#' + id).val() == -1 || $('#' + id).val().trim() == '' || $("#" + id + " option:selected").text() == '') {
        $('#' + id).closest('.form-group').find('.help-block').removeClass('hidden');
        EnableCustomerButton();
    }                
    else {
        $('#' + id).closest('.form-group').find('.help-block').addClass('hidden');
        EnableCustomerButton();
    }
}

function EnableCustomerButton() {
    if ($("#ddlSlabColorID").val() > 0 && $('#SFQuantity').val() > 0 && $("#ddlEdges").val() > 0) {
        $('#ShowCustomerModal').prop('disabled', false);
        $('#pCustomerHelpBlock').addClass('hidden');
    }
    else {
        $('#ShowCustomerModal').prop('disabled', true);
        $('#pCustomerHelpBlock').removeClass('hidden');
    }           
}



$(document).ready(function () {
    // Generate a simple captcha
    function randomNumber(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }
    var qMath = [randomNumber(1, 9), '+', randomNumber(1, 9), '='].join(' ');
    $('#verify_contact').attr("placeholder", qMath);
    $('#verify_contact').attr("data-content", 'Then answer this simple math question: ' + qMath);
    $('#verify_contact').popover();

    $('#customerquote-form').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'icon-ok',
            invalid: 'icon-cancel',
            validating: 'icon-spin2'
        },
        container: 'popover',
        fields: {
            CustomerFirstName: {
                validators: {
                    notEmpty: {
                        message: 'Firstname is required'
                    }
                }
            },
            CustomerLastName: {
                validators: {
                    notEmpty: {
                        message: 'Surname is required'
                    }
                }
            },
            Phone: {
                validators: {
                    notEmpty: {
                        message: 'Phone is required'
                    }
                }
            },
            Email: {
                validators: {
                    notEmpty: {
                        message: 'The email address is required'
                    },
                    emailAddress: {
                        message: 'Not a valid email address'
                    }
                }
            },
            verify_contact: {
                validators: {
                    callback: {
                        message: 'Wrong answer',
                        callback: function (value, validator, $field) {
                            var items = $('#verify_contact').attr("placeholder").split(' '), sum = parseInt(items[0]) + parseInt(items[2]);
                            return value == sum;
                        }
                    }
                }
            }
        }
    })
    .on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0) {    // There is invalid field
            data.fv.disableSubmitButtons(true);
            //$('#submit-quote').attr('disabled', 'disabled');
        }
    })
    .on('success.form.fv', function (e) {
        // Prevent form submission
        e.preventDefault();

        // Get the form instance
        var $form = $(e.target);

        // Get the FormValidation instance
        var bv = $form.data('formValidation');

        /* get some values from elements on the page: */
        var $form = $(this),
            url = $form.attr('action');


        ///* Send the data using post */
        //var posting = $.post(url, {

        //});

        ///* Alerts the results */
        //posting.done(function (data) {
        //    //alert(data);
        //});
    });
});
