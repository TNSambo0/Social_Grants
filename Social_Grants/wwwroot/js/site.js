// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//'use strict'; 'use strict';

//(function ($) {

//})(jQuery);

//jQuery(document).ready(function () {
//    ImgUpload();
//});

//function ImgUpload() {
//    var imgWrap = "";
//    var imgArray = [];

//    $('.upload__inputfile').each(function () {
//        $(this).on('change', function (e) {
//            imgWrap = $(this).closest('.upload__box').find('.upload__img-wrap');
//            var maxLength = $(this).attr('data-max_length');

//            var files = e.target.files;
//            var filesArr = Array.prototype.slice.call(files);
//            var iterator = 0;
//            filesArr.forEach(function (f, index) {

//                if (!f.type.match('image.*')) {
//                    return;
//                }

//                if (imgArray.length > maxLength) {
//                    return false
//                } else {
//                    var len = 0;
//                    for (var i = 0; i < imgArray.length; i++) {
//                        if (imgArray[i] !== undefined) {
//                            len++;
//                        }
//                    }
//                    if (len > maxLength) {
//                        return false;
//                    } else {
//                        imgArray.push(f);

//                        var reader = new FileReader();
//                        reader.onload = function (e) {
//                            var html = "<div class='upload__img-box'><div style='background-image: url(" + e.target.result + ")' data-number='" + $(".upload__img-close").length + "' data-file='" + f.name + "' class='img-bg'><div class='upload__img-close'></div></div></div>";
//                            imgWrap.append(html);
//                            iterator++;
//                        }
//                        reader.readAsDataURL(f);
//                    }
//                }
//            });
//        });
//    });

//    $('body').on('click', ".upload__img-close", function (e) {
//        var file = $(this).parent().data("file");
//        for (var i = 0; i < imgArray.length; i++) {
//            if (imgArray[i].name === file) {
//                imgArray.splice(i, 1);
//                break;
//            }
//        }
//        $(this).parent().parent().remove();
//    });
//}


// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var error = $(".error");
    var iDIsNumber = false;
    $(".IDNumber").on("change", function () {
        var iDNumber = $(this).val();
        if (isNaN(iDNumber)) {
            error.html("Please enter a correct ID number");
        }
        else {
            iDIsNumber = true;
            error.html("");
            if (iDNumber.length >= 6) {
                if (`${iDNumber[2]}${iDNumber[3]}` > "12") {
                    error.html("Please enter a correct ID number");
                }
                else if (`${iDNumber[4]}${iDNumber[5]}` > "31") {
                    error.html("Please enter a correct ID number");
                }
                else {
                    error.html("");
                }
            }
        }
        btnSubmit();
    });
    function btnSubmit() {
        var btnSubmit = $("#BtnSubmit");
        if (error.html() == "" && iDIsNumber && $(".IDNumber").val().length == 13) {
            btnSubmit.prop('disabled', false);
            $(".error").addClass('d-none');
        }
        else {
            btnSubmit.prop('disabled', true);
            $(".error").removeClass('d-none');
        }
    }
    //$(".btn-edit-profile").on("click", function () {
    //    $("input:text").each(function () {
    //        $(this).attr("readonly", false);
    //    });
    //    $("select").attr("readonly", false);
    //    $(".btn-submit-profile")[0].classList.remove("d-none");
    //    $(".btn-edit")[0].classList.add("d-none");
    //});
    var ForWhodropdown = $(".ForWho");
    //Display(ForWhodropdown[0]);
    ForWhodropdown.on("change", function () {
        DisplayDetailsForWho($(this)[0]);
        error.html("");
    });
    $(window).on("load", function () {
        DisplayDependentDitailsDisabilityGrant();
        var dropdown = $(".ForWho")[0];
        DisplayDetailsForWho(dropdown);
    });
    function DisplayDetailsForWho(Select) {
        var Selected = Select.options[Select.selectedIndex].text;
        var dependent_Create_router = $(".dependent_Create_router")[0].classList;
        var dependent_details = $(".dependent_details")[0].classList;
        var applicant_details = $(".applicant_details")[0].classList;
        var dependent_questions = $(".dependent_questions")[0].classList;
        if (Selected == "Myself") {
            dependent_questions.remove("d-none");
            applicant_details.remove("d-none");
            dependent_Create_router.add("d-none");
            dependent_details.add("d-none");
        }
        else if (Selected == "Dependent") {
            applicant_details.remove("d-none");
            dependent_details.remove("d-none");
            DisplayDependentDitails(dependent_questions);
        }
        else {
            applicant_details.add("d-none");
        }
    }
    function DisplayDependentDitails() {
        var dependentsCounts = $(".dependents_count")[0].innerText;
        var dependent_Create_router = $(".dependent_Create_router")[0].classList;
        var sub_dependent_details = $(".sub_dependent_details")[0].classList;
        if (dependentsCounts == "" || dependentsCounts == null) {
            dependent_Create_router.remove("d-none");
            sub_dependent_details.add("d-none");
            if (elementClassList != null || elementClassList != "") {
                elementClassList.add("d-none");
            }
        }
        else {
            dependent_Create_router.add("d-none");
            sub_dependent_details.remove("d-none");
        }
    }
    function DisplayDependentDitailsDisabilityGrant() {
        var dependentsCounts = $(".dependents_count")[0].innerText;
        var dependent_Create_router = $(".dependent_Create_router")[0].classList;
        var dependent_details = $(".dependent_details")[0].classList;
        if (dependentsCounts == "" || dependentsCounts == null) {
            dependent_Create_router.remove("d-none");
            dependent_details.add("d-none");
        }
        else {
            dependent_Create_router.add("d-none");
            dependent_details.remove("d-none");
        }
    }

    $(".FormChecker").on("click", function () {
        var Selected = ForWhodropdown[0].options[ForWhodropdown[0].selectedIndex].text;
        var Selected = ForWhodropdown[0].options[ForWhodropdown[0].selectedIndex].text;
        var FullName = $(".FullName")[0].value;
        var LastName = $(".LastName")[0].value;
        var IDNumber = $(".IDNumber")[0].value;
        var Nationality = $(".Nationality")[0].value;
        var CaredByState = $(".CaredByState")[0].value;
        var OtherGrant = $(".OtherGrant")[0].value;
        var Dependent = GetFileName($(".Dependent")[0].value);
        var MedicalReport = GetFileName($(".MedicalReport")[0].value);
        var ApplicationForm = GetFileName($(".ApplicationForm")[0].value);
        function GetFileName(fullPath) {
            var fname = "";
            if (fullPath) {
                var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
                var filename = fullPath.substring(startIndex);
                if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                    fname = filename.substring(1);
                }
            }
            return fname
        }
        if (Selected == "Myself") {
            if (CaredByState == "" || MedicalReport == "" || ApplicationForm == "") {
                error.html("Please fill all the required fields.");
            }
            else {
                $(".BtnSubmit").click();
            }
        }
        else if (Selected == "Dependent") {
            if (FullName == "" || LastName == "" || IDNumber == "" || Nationality == "" || CaredByState == "" ||
                OtherGrant == "" || Dependent == "" || MedicalReport == "" || ApplicationForm == "") {
                error.html("Please fill all the required fields.");
            }
            else {
                $(".BtnSubmit").click();
            }
        }
        else {
            error.html("Please fill all the required fields.");
        }
    });
    var PaymentMethod = $(".PaymentMethod");
    var Selected;
    $(function () {
        Selected = PaymentMethod[0].options[PaymentMethod[0].selectedIndex].text;
        DisplayMethodOFPayment(Selected);
        error.html("");
    });
    PaymentMethod.on("change", function () {
        Selected = $(this)[0].options[$(this)[0].selectedIndex].text;
        DisplayMethodOFPayment(Selected);
        error.html("");
    });
    function DisplayMethodOFPayment(Selected) {
        var DIVBank = $(".DIVBank");
        var DIVPostOffice = $(".DIVPostOffice");
        var BtnFormChecker = $(".FormChecker");
        if (Selected == "Post Office") {
            DIVPostOffice[0].classList.remove("d-none");
            DIVBank[0].classList.add("d-none");
            BtnFormChecker.prop("disabled", false);
        }
        else if (Selected == "Bank account") {
            DIVBank[0].classList.remove("d-none");
            DIVPostOffice[0].classList.add("d-none");
            BtnFormChecker.prop("disabled", false);
        }
        else {
            DIVBank[0].classList.add("d-none");
            DIVPostOffice[0].classList.add("d-none");
            BtnFormChecker.prop("disabled", true);
        }
    }
    $(".PaymentFormChecker").on("click", function () {
        var BankName = $(".BankName")[0].value;
        var BankAccountHolder = $(".BankAccountHolder")[0].value;
        var AccountNumber = $(".AccountNumber")[0].value;
        var BranchCode = $(".BranchCode")[0].value;
        var POProvince = $(".POProvince :selected").val();
        var POArea = $(".POArea :selected").val();
        var POSite = $(".POSite :selected").val();
        if (Selected == "Post Office") {
            if (POProvince == 0 || POArea == 0 || POSite == 0) {
                error.html("Please fill all the required fields.");
            }
            else {
                $(".Btn-Submit").click();
            }
        }
        else if (Selected == "Bank account") {
            if (BankName == "" || BankAccountHolder == "" || AccountNumber == "" || BranchCode == "") {
                error.html("Please fill all the required fields.");
            }
            else {
                $(".Btn-Submit").click();
            }
        }
        else {
            error.html("Please fill all the required fields.");
        }
    });
    $(".profile-picture").on("click", function () {
        $(".profile-picture-file").click();
    });
    $(".profile-picture-file").on("change", function () {
        var fileUpload = $(".profile-picture-file").get(0);
        var uploadedFile = fileUpload.files;
        var fileData = new FormData();
        fileData.append(uploadedFile[0].name, uploadedFile[0]);
        $.ajax({
            method: "POST",
            url: 'Account/ChangeProfilePicture',
            contentType: false,
            processData: false,
            data: fileData,
            success:
                function (data) {
                    data = JSON.parse(data);
                    if (data.Status == "error") {
                        toastr.error(data.Message);
                    }
                    else if (data.Status == "Login") {
                        toastr.error(data.Message);
                        window.location.href = "Account/Login";
                    }
                    else {
                        $(".profile-picture").attr("src", data.Message);
                    }
                },
            error:
                function (err) {
                    toastr.error('error');
                }
        });
    });
});