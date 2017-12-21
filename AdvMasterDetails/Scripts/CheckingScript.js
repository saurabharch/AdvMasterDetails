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
function getFdate() {
    var date = $('#FromDate').val().trim();
    return date;
}

function getTdate() {
    var date = $('#ToDate').val().trim();
    return date;
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
                    FromDate: getFdate(),
                    ToDate: getTdate(),
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

        if (isAllValid) {
            var data = {
                OrderDetails: list
            }

            $(this).val('Please wait...');
            var  FromDate= getFdate();
            var ToDate = getTdate();
            //var ProductID = 35;
            //var Quantity = 2;
            var States = "Approved - Package";
            //var ProductID = $('select.product', this).val();
            //var Quantity = parseInt($('.quantity', this).val());
            //** Example Link : http://localhost:47367/home/CheckAvailiblity?fromDate=17%20November%202017%2006:00%20am&ToDate=23%20November%202017%2009:00%20am&ProdId=35&state=Approved%20-%20Package&Quantity=3 **//
            for (var i = 0; i < list.length - 1; i++) {
                var createStringP = list.product[i];
                var createStringQ = list.quantity[i];
                $.ajax({
                    type: 'GET',
                    url: '/home/CheckAvailiblity?fromDate=' + FromDate + '&ToDate=' + ToDate + '&ProdId=' + createStringP + '&state=Aprroved%20-%20Package&Quantity=' + createStringQ,
                    data: JSON.stringify(data),
                    contentType: 'application/json',
                    success: function (data) {
                        if (data.status) {
                            alert(data.val);
                            //here we will clear the form
                           // list = [];
                            //$('#FromDate', '#ToDate', '#product', '#quantity').val('');
                            //$('#orderdetailsItems').empty();
                            console.log(data.val);
                        }
                        else {
                            console.log(data)
                            alert('Error');

                        }
                        $('#submit').text('Check Availiblity');
                        $(this).val('Availiblity');
                    },
                    error: function (error) {
                        console.log(error);
                        $('#submit').text('Check Availiblity');
                    }
                });
            }

        
        }

    });
});
LoadCategory($('#productCategory'));