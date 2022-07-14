// Write your Javascript code.

var highlightFields = function (response) {

    $('.form-group').removeClass('has-error');

    $.each(response.errors, function (index, error) {
        //var nameSelector = '[name = "' + propName.replace(/(:|\.|\[|\])/g, "\\$1") + '"]',
        //    idSelector = '#' + propName.replace(/(:|\.|\[|\])/g, "\\$1");
        //var $el = $(nameSelector) || $(idSelector);

        //if (val.Errors.length > 0) {
        //    $el.closest('.form-group').addClass('has-error');
        //}
        var $el = $(error.field);
        if (error.message.length > 0) {
            $el.closest('.form-group').addClass('has-error');
        }
    });
};
var highlightErrors = function (xhr) {
    try {
        var data = JSON.parse(xhr.responseText);
        highlightFields(data);
        showSummary(data);
        window.scrollTo(0, 0);
    } catch (e) {
        // (Hopefully) caught by the generic error handler in `config.js`.
    }
};
var showSummary = function (response) {
    $('#validationSummary').empty().removeClass('hidden');
    var $ul = $('#validationSummary').append('<ul class="list-group"></ol>');
    $.each(response.errors, function (index, error) {
        var $li = $('<li class="list-group-item list-group-item-danger"></li>').text(error.message);
        $li.appendTo($ul);
    });
};

var redirect = function (data) {
    if (data.redirect) {
        window.location = data.redirect;
    } else {
        window.scrollTo(0, 0);
        window.location.reload();
    }
};

//$('form[method=post]').not('.no-ajax').on('submit', function () {
//    var submitBtn = $(this).find('[type="submit"]');

//    submitBtn.prop('disabled', true);
//    $(window).unbind();

//    var $this = $(this),
//        formData = $this.serialize();

//    $this.find('div').removeClass('has-error');

//    $.ajax({
//        url: $this.attr('action'),
//        type: 'post',
//        data: formData,
//        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
//        dataType: 'json',
//        statusCode: {
//            200: redirect
//        },
//        complete: function () {
//            submitBtn.prop('disabled', false);
//        },
//        error: function(xhr, textStatus, errorThrown) {
//            console.log('jqXHR:');
//            console.log(xhr);
//            console.log('textStatus:');
//            console.log(textStatus);
//            console.log('errorThrown:');
//            console.log(errorThrown);
//            highlightErrors(xhr);
//        }
//    });

//    return false;
//});