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

function LoadProductavl() {
    var data = [];
    var fmdt = getFdate();
    var tmdt = getTdate();
    var productid = $('#product').val();
    var quantity = $('#quantity').val();
    var isThisValid = true;
    var ItemCount = [];
    $('#orderItemError').text('');
    var list = [];
    var errorItemCount = 0;
    if ($('#FromDate').val().trim() == '') {
        $('#FromDate').siblings('span.error').css('visibility', 'visible');
        isThisValid = false;
    }
    else {
        $('#FromDate').siblings('span.error').css('visibility', 'hidden');
    }
    if ($('#ToDate').val().trim() == '') {
        $('#ToDate').siblings('span.error').css('visibility', 'visible');
        isThisValid = false;
    }
    else {
        $('#ToDate').siblings('span.error').css('visibility', 'hidden');
    }
    if (isThisValid) {
        $.ajax({
            context: $(this),
            type: 'POST',
            url: '/home/ProdCheckAvailiblity?fromDate=' + fmdt + '&ToDate=' + tmdt + '&ProdId=' + productid + '&state=Aprroved%20-%20Package&Quantity=' + quantity,
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (data) {

                $("#orderItemBadge").html(data.AVProdCount);
                if (data.AVProdCount >= quantity) {
                    $("#orderItemBadge").addClass('badge-success').removeClass('badge-danger badge-default');
                }
                else {
                    $("#orderItemBadge").addClass('badge-danger').removeClass('badge-success badge-default');
                }
            },
            error: function (error) {
                console.log(error);
               
            }
        })
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


$(document).ready(function () {
    //Add button click event
    var counter = 1;
    $('#add').click(function () {
        counter += 1;
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
            var $badge = $(this).find('.badge'),
                     count = Number($badge.text());
            
            $("#orderItemBadge").html(0);
            
            $('.remove').width('100px');
            //remove id attribute from new clone row
            $('#productCategory,#product,#quantity,#add', $newRow).removeAttr('id');
            $badge.addClass('.bagde-success').removeClass('.badge-default');
            // $badge.atrr('id', counter);
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

    //$('#submit').click(function () {
    //    var isAllValid = true;
    //    var ItemCount = [];
    //    $('#orderItemError').text('');
    //    var list = [];
    //    var errorItemCount = 0;
    //    $('#orderdetailsItems tbody tr').each(function (index, ele) {
    //        if (
    //            $('select.product', this).val() == "0" ||
    //            (parseInt($('.quantity', this).val()) || 0) == 0
    //            ) {
    //            errorItemCount++;
    //            $(this).addClass('error');
    //        } else {
    //            var orderItem = {
    //                ProductID: $('select .product', this).val(),
    //                Quantity: parseInt($('.quantity', this).val()),
    //                FromDate: getFdate(),
    //                ToDate: getTdate()
    //            }
    //            list.push(orderItem);
    //        }
    //    })

    //    if (errorItemCount > 0) {
    //        $('#orderItemError').text(errorItemCount + " invalid entry in order item list.");
    //        isAllValid = false;
    //    }

    //    if (list.length == 0) {
    //        $('#orderItemError').text('At least 1 order item required.');
    //        isAllValid = false;
    //    }
    //    if ($('#FromDate').val().trim() == '') {
    //        $('#FromDate').siblings('span.error').css('visibility', 'visible');
    //        isAllValid = false;
    //    }
    //    else {
    //        $('#FromDate').siblings('span.error').css('visibility', 'hidden');
    //    }
    //    if ($('#ToDate').val().trim() == '') {
    //        $('#ToDate').siblings('span.error').css('visibility', 'visible');
    //        isAllValid = false;
    //    }
    //    else {
    //        $('#ToDate').siblings('span.error').css('visibility', 'hidden');
    //    }

    //    if (isAllValid) {
    //        var data = {
    //            OrderDetails: list
    //        }

    //        $(this).val('Please wait...');
    //        var FromDate = getFdate();
    //        var ToDate = getTdate();
    //        //var ProductID = 35;
    //        //var Quantity = 2;
    //        var States = "Approved - Package";

    //        //var ProductID = $('select.product', this).val();
    //        //var Quantity = parseInt($('.quantity', this).val());
    //        var i = 0;
    //        var labelValue = 0;
    //        var promises = [];
    //        //** Example Link : http://localhost:47367/home/CheckAvailiblity?fromDate=17%20November%202017%2006:00%20am&ToDate=23%20November%202017%2009:00%20am&ProdId=35&state=Approved%20-%20Package&Quantity=3 **//
    //        for (i; i < list.length; i++) {
    //            //var createStringP = list[i].ProductID;
    //            //var createStringQ = list[i].Quantity;
    //            //console.log(list[i]);
    //            //var quantity = list[i].Quantity.val();

    //            promises.push($.ajax({
    //                context: $(this),
    //                type: 'POST',
    //                url: '/home/ProdCheckAvailiblity?fromDate=' + list[i].FromDate + '&ToDate=' + list[i].ToDate + '&ProdId=' + list[i].ProductID + '&state=Aprroved%20-%20Package&Quantity=' + list[i].Quantity,
    //                data: JSON.stringify(data),
    //                contentType: 'application/json',
    //                success: function (data) {
    //                    var getid = 1;
    //                    for (var j = 0; j < 1; j++) {

    //                        var id = getid;
    //                        //alert(data.AVProdCount);
    //                        alert(id);
    //                        //alert($("#1").html());
    //                        $("#" + id).html(data.AVProdCount);
    //                        getid += 1;


    //                        console.log(data);
    //                    }

    //                    // }
    //                    //else {
    //                    //    console.log(response);

    //                    ////$("#add").append("<span class='badge badge-pill badge-success'> - 2nd to last!</span>")
    //                    //  alert('Error');

    //                    //  }

    //                    $('#submit').text('Check Availiblity');
    //                    $(this).val('Availiblity');
    //                },
    //                error: function (error) {
    //                    console.log(error);
    //                    $('#submit').text('Check Availiblity');
    //                }
    //            }))
    //        }
    //        //console.log(ItemCount);
    //        if (ItemCount.length > 0) {

    //            //for (var h = 0; h <= ItemCount.length; h++) {
    //            //    console.log(ItemCount[h].text);
    //            //    var pCount=  parseInt($('.quantity', this).val());
    //            //    if (ItemCount[h].val <= pCount)
    //            //    {
    //            //        $("#add").append("<span class='badge badge-pill badge-success'>"+ItemCount[h].text+"</span>")
    //            //    }
    //            //    else{
    //            //        $("#add").append("<span class='badge badge-pill badge-danger'>" + ItemCount[h].text + "</span>")
    //            //    }
    //            //}
    //        }
    //    }

    //});
});
LoadCategory($('#productCategory'));