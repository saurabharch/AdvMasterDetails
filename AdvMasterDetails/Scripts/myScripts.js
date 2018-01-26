var Categories = []
//fetch categories from database
function LoadCategory(element) {
    if (Categories.length == 0) {
        //ajax function for fetch data
        $.ajax({
            type: "GET",
            url: '/home/getProductCategories',
            success: function (data) {
                Categories = data;
                //render catagory
                renderCategory(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderCategory(element);
    }
}

function getFdate() {
    var date = $('#FromDate').val().trim();
    return date;
}

function getTdate() {
    var date = $('#ToDate').val().trim();
    return date;
}

function renderCategory(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(Categories, function (i, val) {
        $ele.append($('<option/>').val(val.CategoryID).text(val.CategortyName));
    })
}
//fetch products

function renderProduct(element, data) {
    //render product
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(data, function (i, val) {
        $ele.append($('<option/>').val(val.ProductID).text(val.ProductName));
    })
}
function LoadProduct(categoryDD) {
    $.ajax({
        type: "GET",
        url: "/home/getProducts",
        data: { 'categoryID': $(categoryDD).val() },
        success: function (data) {
            //render products to appropriate dropdown
            renderProduct($(categoryDD).parents('.mycontainer').find('select.product'), data);
        },
        error: function (error) {
            console.log(error);
        }
    })
}
function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
}



function CurrentDateTime() {
    var d = new Date();
    var days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var month = ["January", "Febrary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    return d.getDate() + " " + days[d.getDay()] + " " + month[d.getMonth()] + " " + d.getFullYear() + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
}

function gettoken() {
    //var token = $('[name=__RequestVerificationToken]').val();
    //return token;
}
$(document).ready(function () {
    //Add button click event
    $('#add').click(function () {
        //validation and add order items
        var isAllValid = true;
        if ($('#productCategory').val() == "0") {
            isAllValid = false;
            $('#productCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#productCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#product').val() == "0") {
            isAllValid = false;
            $('#product').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#product').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#quantity').val().trim() != '' && (parseInt($('#quantity').val()) || 0))) {
            isAllValid = false;
            $('#quantity').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#quantity').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val($('#productCategory').val());
            $('.product', $newRow).val($('#product').val());

            //Replace add button with remove button
            $('#add', $newRow).addClass('remove').val('⛔ Remove').removeClass('btn-success');
            $('.remove').width('100px');
            //remove id attribute from new clone row
            $('#productCategory,#product,#quantity,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            //append clone row
            $('#orderdetailsItems').append($newRow);

            //clear select data
            $('#productCategory,#product').val('0');
            $('#quantity').val('');
            $('#orderItemError').empty();
        }

    })

    //remove button click event
    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).parents('tr').remove();
    });

    $('#submit').click(function () {
        var isAllValid = true;

        //validate order items
        $('#orderItemError').text('');
        var list = [];
        var errorItemCount = 0;
        $('#orderdetailsItems tbody tr').each(function (index, ele) {
            if (
                $('select.product', this).val() == "0" ||
                (parseInt($('.quantity', this).val()) || 0) == 0 
                ) {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var orderItem = {
                    ProductID: $('select.product', this).val(),
                    Quantity: parseInt($('.quantity', this).val()),
                    GUID: guid(),
                    FromDate: getFdate(),
                    ToDate: getTdate(),
                    Status: 'wait for review',
                    Remarks: 'Wait for approval',
                }
                list.push(orderItem);
            }
        })

        if (errorItemCount > 0) {
            $('#orderItemError').text(errorItemCount + " invalid entry in order item list.");
            isAllValid = false;
        }

        if (list.length == 0) {
            $('#orderItemError').text('At least 1 order item required.');
            isAllValid = false;
        }

        if ($('#orderNo').val().trim() == '') {
            $('#orderNo').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#orderNo').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#Email').val().trim() == '') {
            $('#Email').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#Email').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#FromDate').val().trim() == '') {
            $('#FromDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#FromDate').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#ToDate').val().trim() == '') {
            $('#ToDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#ToDate').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#orderDate').val().trim() == '') {
            $('#orderDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#orderDate').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#phone').val().trim() == '') {
            $('#phone').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#phone').siblings('span.error').css('visibility', 'hidden');
        }
        if ($('#company').val().trim() == '') {
            $('#company').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#company').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                OrderNo: $('#orderNo').val().trim(),
                OrderDateString: $('#orderDate').val().trim(),
                Email: $('#Email').val().trim(),
                FromDate: getFdate(),
                ToDate: getTdate(),
                ContactNo: $('#phone').val().trim(),
                Company:$('#company').val().trim(),
                OrderDetails: list
            }

            $(this).val('Please wait...');
            var verificationToken = $('[name=__RequestVerificationToken]').val();
            $.ajax({
                type: 'POST',
                headers: verificationToken,
                url: '/home/save',
                data: JSON.stringify(data),
                cache: false,
                async: true,
                contentType: 'application/json',
                success: function (data) {
                 if (data.status) {
                       // alert('Successfully Placed Order');
                        //here we will clear the form
                     list = [];
                     window.location.reload();
                        $('#orderNo,#orderDate,#Email', '#FromDate', '#ToDate', '#orderDate', '#phone', '#company').val('');
                        $('#orderdetailsItems').empty();
                        $("#myModal").modal('show');
                    }
                    else {
                       // console.log(error)
                      //  alert('Error');
                        
                    }
                    $('#submit').text('Save');
                    $(this).val('Save');
                },
                error: function (error) {
                    $('#myModalError').modal('show');
                    $('#submit').text('Save');
                }
            });
        }
        else {
            $('#myModalError').modal('show');
        }

    });

});

LoadCategory($('#productCategory'));

