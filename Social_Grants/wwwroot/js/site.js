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
    $(".btn-edit-profile").on("click", function () {
        $("input:text").each(function () {
            $(this).attr("readonly", false);
        });
        $("select").attr("readonly", false);
        $(".btn-submit-profile")[0].classList.remove("d-none");
        $(".btn-edit")[0].classList.add("d-none");
    });
    var ForWhodropdown = $(".ForWho");
    Display(ForWhodropdown[0]);
    ForWhodropdown.on("change", function () {
        Display($(this)[0]);
        error.html("");
    });
    function Display(Select) {
        var Selected = Select.options[Select.selectedIndex].text;
        var Details = $(".Details");
        var SubDetails = $(".SubDetails");
        var FileUploadDepenentID = $(".FileUploadDepenentID");
        var SupportingDoc = $(".SupportingDoc");
        if (Selected == "Myself") {
            Details[0].classList.remove("d-none");
            SubDetails[0].classList.add("d-none");
            FileUploadDepenentID[0].classList.add("d-none");
        }
        else if (Selected == "Dependent") {
            Details[0].classList.remove("d-none");
            SubDetails[0].classList.remove("d-none");
            FileUploadDepenentID[0].classList.remove("d-none");
        }
        else {
            Details[0].classList.add("d-none");
        }
    }
    $(".FormChecker").on("click", function () {
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
        else
        {
            error.html("Please fill all the required fields.");
        }
    });
    var PaymentMethod = $(".PaymentMethod");
    var Selected;
    $(function () {
        Selected = PaymentMethod[0].options[PaymentMethod[0].selectedIndex].text;
        Display(Selected);
        error.html("");
    });
    PaymentMethod.on("change", function () {
        Selected = $(this)[0].options[$(this)[0].selectedIndex].text;
        Display(Selected);
        error.html("");
    });
    function Display(Selected) {
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
    $(".FormChecker").on("click", function () {
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
});