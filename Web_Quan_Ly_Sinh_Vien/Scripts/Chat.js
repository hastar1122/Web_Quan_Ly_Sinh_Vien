var imageUser = $('.img-profile').attr('src');
var isNhom = false;
var isChatNhom = false;
var groupname = '';

// Mở modal tìm kiếm người dùng để thêm vào nhóm
$('body').on('click', '.openModalAddUserToGroup', function () {
    var Id = $(this).attr('data-id');
    $('#addUserToGroup').modal('show');
    $('#lstUserToAddGroup').empty();
    $('.find2').attr('data-id', Id);
    $('.addUsers').attr('data-id', Id);
    $('.text-find2').val('');
});

// Mở modal xem danh sách thành viên trong nhóm
$('body').on('click', '.openModalListUserInGroup', function () {
    var Id = $(this).attr('data-id');
    if ($(this).attr('is-admin') == 1) {
        $('.deleteUser').show();
    }
    else {
        $('.deleteUser').hide();
    }
    $('#listUserInGroup').modal('show');
    LoadUserInGroup(Id);
    $('.find3').attr('data-id', Id);
    $('.deleteUser').attr('data-id', Id);
    $('.text-find3').val('');
});

// Mở modal xóa nhóm chat
$('body').on('click', '.openModalDeleteGroup', function () {
    $('#delGroup').modal('show');
    $('.Xoa1').val($(this).attr('data-id'));
});

// Mở modal xóa cuộc trò chuyện
$('body').on('click', '.openModalDeleteMsg', function () {
    $('#delMsg').modal('show');
    $('.Xoa2').val($(this).attr('data-id'));
});

// Mở modal rời khỏi nhóm chat
$('body').on('click', '.openModalLeaveGroup', function () {
    $('#leaveGroup').modal('show');
    $('.leave').val($(this).attr('data-id'));
});

// Xóa tin nhắn riêng
$('body').on('click', '.Xoa2', function () {
    var Id = $(".Xoa2").val();
    $.ajax({
        async: true,
        url: '/Chat/DeleteMsg',
        data: { Id: Id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            if (data.status == true) {
                LoadListNguoiNhan();
                $('.LoadAllTinNhan').empty();
                $('.LoadThongTinTinNhan').empty();
                $('.chatbox').addClass('d-none');
                $('#message').attr('data-id', '');
                toastr.options.positionClass = "toast-bottom-right";
                toastr.success("Xóa thành công");
            }
        },
        error: function () {
            toastr.options.positionClass = "toast-bottom-right";
            toastr.warning('Xóa không thành công');
        }
    });
});

// Rời khỏi nhóm chat
$('body').on('click', '.leave', function () {
    var Id = $(".leave").val();
    $.ajax({
        async: true,
        url: '/Chat/LeaveGroupChat',
        data: { Id: Id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            if (data.status == true) {
                LoadListNhomChat();
                $('.LoadAllTinNhan').empty();
                $('.LoadThongTinTinNhan').empty();
                $('.chatbox').addClass('d-none');
                $('#message').attr('data-id', '');
                toastr.options.positionClass = "toast-bottom-right";
                toastr.success("Rời nhóm thành công");
            }
            if (data.status == false) {
                alert("Không thể xóa giảng viên này");
                toastr.options.positionClass = "toast-bottom-right";
                toastr.warning("Rời nhóm không thành công");
            }
        },
        error: function () {
            toastr.options.positionClass = "toast-bottom-right";
            toastr.warning('Xóa không thành công');
        }
    });
});

// Xóa nhóm chat
$('body').on('click', '.Xoa1', function () {
    var Id = $(".Xoa1").val();
    $.ajax({
        async: true,
        url: '/Chat/DeleteGroupChat',
        data: { Id: Id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            if (data.status == true) {
                LoadListNhomChat();
                $('.LoadAllTinNhan').empty();
                $('.LoadThongTinTinNhan').empty();
                $('.chatbox').addClass('d-none');
                $('#message').attr('data-id', '');
                toastr.options.positionClass = "toast-bottom-right";
                toastr.success("Xóa thành công");
            }
        },
        error: function () {
            toastr.options.positionClass = "toast-bottom-right";
            toastr.warning('Xóa không thành công');
        }
    });
});

// Tìm kiếm người dùng để thêm vào nhóm
$('.find2').on('click', function () {
    var Id = $(this).attr('data-id');
    var s = $('.text-find2').val();
    if (s.trim() == '' || s == null) {
        alert("Vui lòng nhập mã hoặc tên để tìm kiếm");
        $('.text-find2').val('').focus();
        return;
    }
    GetUsersToAddGroup(Id, s);
});

// Tìm kiếm người trong nhóm chat
$('.find3').on('click', function () {
    var Id = $(this).attr('data-id');
    var s = $('.text-find3').val();
    LoadUserInGroup(Id, s);
});

// Hàm load user trong nhóm
function LoadUserInGroup(Id, s) {
    $.ajax({
        url: '/Chat/LoadUserInGroup',
        data: { Id: Id, s: s },
        type: 'GET',
        dataType: 'html',
        success: function (data) {
            $('#LoadUserInGroup').empty();
            $('#LoadUserInGroup').html(data);
        }
    });
}

// Hàm load user để thêm vào nhóm
function GetUsersToAddGroup(Id, s) {
    $.ajax({
        url: '/Chat/GetUsersToAddGroup',
        data: { Id: Id, s: s },
        dataType: 'html',
        type: 'GET',
        success: function (data) {
            $("#lstUserToAddGroup").empty();
            $("#lstUserToAddGroup").append(data);
        }
    });
};

// Thêm người dùng vào nhóm chat
$('.addUsers').on('click', function () {
    var Id = $(this).attr('data-id');
    var s = $('.text-find2').val();
    var selected = [];
    $('#lstUserToAddGroup input:checked').each(function () {
        selected.push($(this).attr('id'));
    });
    if (selected.length > 0) {
        $.ajax({
            url: '/Chat/AddUserToGroup',
            type: 'POST',
            dataType: 'json',
            data: { Id: Id, UserId: selected },
            async: true,
            success: function (data) {
                if (data.status == true) {
                    GetUsersToAddGroup(Id, s);
                    toastr.options.positionClass = "toast-bottom-right";
                    toastr.success("Thêm mới thành công");
                }
            }
        });
    }
    
});

// Xóa người dùng khỏi nhóm chat
$('.deleteUser').on('click', function () {
    var Id = $(this).attr('data-id');
    var s = $('.text-find3').val();
    var selected = [];
    $('#LoadUserInGroup input:checked').each(function () {
        selected.push($(this).attr('id'));
    });
    if (selected.length > 0) {
        $.ajax({
            url: '/Chat/DeleteUserFromGroup',
            type: 'POST',
            dataType: 'json',
            data: { Id: Id, UserId: selected },
            async: true,
            success: function (data) {
                if (data.status == true) {
                    LoadUserInGroup(Id, s);
                    toastr.options.positionClass = "toast-bottom-right";
                    toastr.success("Xóa thành công");
                }
            }
        });
    }
});

$("#search").on("keyup", function () {
    var value = $(this).val().trim().toLowerCase();
    //if (value != "") {
    //    $('#search').addClass("active");
    //}
    //else {
    //    $('#search').removeClass("active");
    //}
    if (isNhom == false) {
        $("#listMessage li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    }
    else if (isNhom == true) {
        $("#listMessage2 li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    }
});

// Mở modal thông tin nhóm chat
$('body').on('click', '.openModalInforGroup', function () {
    var Id = $(this).attr('data-id');
    $.ajax({
        url: '/Chat/LoadInfoNhom',
        data: { Id: Id },
        dataType: 'json',
        type: 'GET',
        success: function (data) {
            $.each(data.data, function (index, row) {
                $("#Id2").val(row.Id);
                $("#TenNhom2").val(row.TenNhom);
                $("#Ten2").val(row.Ten);
                $(".avatar2").attr('src', row.Avatar);
                $("#Avatar2").val(row.Avatar);
                $(".loadtext2").html(row.Avatar.split('/').pop());
            });
        }
    });
    $('#infoGroup').modal('show');
});

//setInterval(() => {
//    if (!$('#search').hasClass('active') && isNhom == false) {
//        LoadListNguoiNhan();
//    }
//    else if(!$('#search').hasClass('active') && isNhom == true){
//        LoadListNhomChat();
//    }
//}, 5000);

// Create nhóm chat
$('body').on('click', '.create', function () {
    var formdata = new FormData();
    formdata.append("TenNhom", $("#TenNhom").val());
    formdata.append("ImageFile", $("#customFile").get(0).files[0]);
    if ($("#TenNhom").val().trim() == '') {
        alert('Vui lòng nhập tên nhóm');
        $("#TenNhom").val('');
        $("#TenNhom").focus();
    }
    $.ajax({
        async: true,
        url: '/Chat/CreateNhomChat',
        data: formdata,
        contentType: false,
        processData: false,
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            if (data.status == true) {
                LoadListNhomChat();
                toastr.options.positionClass = "toast-bottom-right";
                toastr.success("Thêm mới thành công");
                $('#addModal').modal('hide');
                $("#TenNhom").val('');
                $("#customFile").val('');
                $('.avatar').attr('src', "/Images/NhomChat/avatar.png");
                $(".loadtext").html("Chọn file");
            }
            else {
                toastr.options.positionClass = "toast-bottom-right";
                toastr.warning('Thêm mới không thành công');
            }
        },
        error: function () {
            toastr.options.positionClass = "toast-bottom-right";
            toastr.warning('Thêm mới không thành công');
        }
    });
});

// Update thông tin nhóm chat
$(".update").click(function () {
    var formdata = new FormData();
    formdata.append("Id", $("#Id2").val());
    formdata.append("TenNhom", $("#TenNhom2").val());
    formdata.append("Avatar", $("#Avatar2").val());
    formdata.append("ImageFile", $("#customFile2").get(0).files[0]);
    $.ajax({
        async: true,
        url: '/Chat/UpdateThongTinNhom',
        contentType: false,
        processData: false,
        data: formdata,
        dataType: 'json',
        type: 'POST',
        success: function (data) {
            if (data.status == true) {
                $.ajax({
                    url: '/Chat/LoadThongTinNhom',
                    data: { id: $("#Id2").val() },
                    dataType: 'html',
                    type: 'GET',
                    success: function (data) {
                        $('.LoadThongTinTinNhan').html(data);
                    }
                });
                LoadListNhomChat();
                toastr.options.positionClass = "toast-bottom-right";
                toastr.success("Chỉnh sửa thành công");
                $('#infoGroup').modal('hide');
            }
            else {
                alert(data.status);
                toastr.options.positionClass = "toast-bottom-right";
                toastr.warning('Chỉnh sửa không thành công');
            }
        },
        error: function () {
            toastr.options.positionClass = "toast-bottom-right";
            toastr.warning('Chỉnh sửa không thành công');
        }
    });
});

// Load danh sách người nhận
$('body').on('click', '.LoadListNguoiNhan', function () {
    isNhom = false;
    $('.openModal').attr('data-target', '#lstUserModal');
    LoadListNguoiNhan();
});

// Load danh sách nhóm chat
$('body').on('click', '.LoadListNhomChat', function () {
    isNhom = true;
    $('.openModal').attr('data-target', '#addModal');
    LoadListNhomChat();
});

$(function () {
    var chat = $.connection.chatHub;

    chat.client.ThongBaoOnline = function (Id) {
        if (isNhom == false) {
            $('#listMessage').find('.user-' + Id).removeClass('chat-offline').addClass('chat-online');
        }
    };

    chat.client.ThongBaoOffline = function (Id) {
        if (isNhom == false) {
            $('#listMessage').find('.user-' + Id).removeClass('chat-online').addClass('chat-offline');
        }
    };

    chat.client.ThongBaoUpdateThongTinNhom = function (GroupId, status) {
        if (GroupId == $('#message').attr('data-id') && status == true && isChatNhom == true) {
            LoadListNhomChat();
            $.ajax({
                url: '/Chat/LoadThongTinNhom',
                data: { id: GroupId },
                dataType: 'html',
                type: 'GET',
                success: function (data) {
                    $('.LoadThongTinTinNhan').html(data);
                }
            });
        }
        else {
            LoadListNhomChat();
        }
    };

    chat.client.message = function (fromUser, message) {
        if (fromUser == $('#message').attr('data-id') && isChatNhom == false) {
            $('.chat-messages').append(message);
            console.log('đâsdas');
            LoadListNguoiNhan();
        }
        else if (isNhom == false) {
            LoadListNguoiNhan();
        }
    };

    chat.client.uploadfile = function (fromUser, message) {
        if (fromUser == $('#message').attr('data-id') && isChatNhom == false) {
            $('.chat-messages').append(message);
            LoadListNguoiNhan();
        }
        else if (isNhom == false) {
            LoadListNguoiNhan();
        }
    };

    chat.client.ThongBaoAddVaoNhom = function (status) {
        if (status == true) {
            LoadListNhomChat();
        }
    };

    chat.client.ThongBaoXoaTinNhan = function (userId, status) {
        if (userId == $('#message').attr('data-id') && status == true && isChatNhom == false) {
            LoadListNguoiNhan();
            $('.LoadAllTinNhan').empty();
            $('.LoadThongTinTinNhan').empty();
            $('.chatbox').addClass('d-none');
            $('#message').attr('data-id', '');
        }
        else {
            LoadListNguoiNhan();
        }
    };

    chat.client.ThongBaoBiKickKhoiNhom = function (Group, status) {
        if (Group == $('#message').attr('data-id') && status == true) {
            LoadListNhomChat();
            $('.LoadAllTinNhan').empty();
            $('.LoadThongTinTinNhan').empty();
            $('.chatbox').addClass('d-none');
            $('#message').attr('data-id', '');
        }
        else {
            LoadListNhomChat();
        }
    };

    chat.client.ThongBaoXoaNhomChat = function (Group, status) {
        if (Group == $('#message').attr('data-id') && status == true) {
            LoadListNhomChat();
            $('.LoadAllTinNhan').empty();
            $('.LoadThongTinTinNhan').empty();
            $('.chatbox').addClass('d-none');
            $('#message').attr('data-id', '');
        }
        else {
            LoadListNhomChat();
        }
    };

    chat.client.messageGroup = function (fromUser, message) {
        if (fromUser == $('#message').attr('data-id') && isChatNhom == true) {
            $('.chat-messages').append(message);
            LoadListNhomChat();
        }
        else if (isNhom == true) {
            LoadListNhomChat();
        }
    };

    chat.client.uploadfileGroup = function (fromUser, message) {
        if (fromUser == $('#message').attr('data-id') && isChatNhom == true) {
            $('.chat-messages').append(message);
            LoadListNhomChat();
        }
        else if (isNhom == true) {
            LoadListNhomChat();
        }
    };

    // Set initial focus to message input box.
    $('#message').focus();

    // Start the connection.
    $.connection.hub.start().done(function () {

        $('body').on('click', '.connect', function () {
            var btn = $(this);
            isChatNhom = false;
            $('#message').attr('data-id', btn.attr('data-id'));
            $('#sendmessage').attr('data-id', btn.attr('data-id'));
            LoadThongTinTinNhan(btn.attr('data-id'));
            $('#message').focus();
        });

        $('body').on('click', '.connect2', function () {
            var btn = $(this);
            isChatNhom = true;
            //if (groupname != '') {
            //    chat.server.remove(groupname);
            //}
            //groupname = btn.attr('data-id');
            //chat.server.join(groupname);
            $('#message').attr('data-id', btn.attr('data-id'));
            $('#sendmessage').attr('data-id', btn.attr('data-id'));
            LoadThongTinTinNhan(btn.attr('data-id'));
            $('#message').focus();
        });

    });
});

function SendMessage(element, e) {
    let message = $(element).val();
    let toUserId = $(element).attr('data-id');
    if (e.which === 13) {
        $.ajax({
            url: '/Chat/SendMessage',
            type: 'POST',
            dataType: 'html',
            data: { toUserId: toUserId, message: message, isChatNhom: isChatNhom },
            async: true,
            success: function (data) {
                $('.LoadAllTinNhan').append(data);
                $(element).val('').focus();
            },
            complete: function () {
                if (isChatNhom == true) {
                    LoadListNhomChat();
                }
                else {
                    LoadListNguoiNhan();
                }
            }
        });
    };
}

// Khi modal tìm kiếm người dùng được hiện lên
$("#lstUserModal").on('show.bs.modal', function () {
    $("#lstUser").empty();
    $('.text-find').val('');
});

// Tìm kiếm người dùng để tạo tin nhắn mới
$('.find').on('click', function () {
    var s = $('.text-find').val();
    if (s.trim() == '' || s == null) {
        alert("Vui lòng nhập mã hoặc tên để tìm kiếm");
        $('.text-find').val('').focus();
        return;
    }
    $.ajax({
        url: '/Chat/GetUsers',
        data: { s: s },
        dataType: 'html',
        type: 'GET',
        success: function (data) {
            $("#lstUser").empty();
            $("#lstUser").append(data);
        }
    });
});

// Tạo tin nhắn riêng mới
$('body').on('dblclick', '.create-msg', function () {
    var id = $(this).attr('data-id');
    $('#message').attr('data-id', id);
    $('#sendmessage').attr('data-id', id);
    isChatNhom = false;
    LoadThongTinTinNhan(id);
    $('#lstUserModal').modal('hide');
    $('#message').focus();
});

// Load thông tin tin nhắn và các tin nhắn
function LoadThongTinTinNhan(id) {
    if (isChatNhom == false) {
        $.ajax({
            url: '/Chat/LoadThongTinTinNhan',
            data: { id: id },
            dataType: 'html',
            type: 'GET',
            success: function (data) {
                $('.LoadThongTinTinNhan').html(data);
            }
        });
        $.ajax({
            url: '/Chat/LoadAllTinNhanRieng',
            data: { id: id },
            dataType: 'html',
            type: 'GET',
            success: function (data) {
                $('.LoadAllTinNhan').html(data);
            }
        });
        $('.chatbox').removeClass('d-none');
        $('#lstUserModal').modal('hide');
    }
    else {
        $.ajax({
            url: '/Chat/LoadThongTinNhom',
            data: { id: id },
            dataType: 'html',
            type: 'GET',
            success: function (data) {
                $('.LoadThongTinTinNhan').html(data);
            }
        });
        $.ajax({
            url: '/Chat/LoadAllTinNhanNhom',
            data: { id: id },
            dataType: 'html',
            type: 'GET',
            success: function (data) {
                $('.LoadAllTinNhan').html(data);
            }
        });
        $('.chatbox').removeClass('d-none');
        $('#lstUserModal').modal('hide');
    }
};

function LoadListNhomChat() {
    $.ajax({
        url: '/Chat/LoadListNhomChat',
        type: 'GET',
        dataType: 'html',
        success: function (data) {
            $('.list-message2').html(data);
        }
    });
};

function LoadListNguoiNhan() {
    $.ajax({
        url: '/Chat/LoadListTinNhanRieng',
        type: 'GET',
        dataType: 'html',
        success: function (data) {
            $('.list-message').html(data);
        }
    });
};

// Tự động scroll xuống cuối trang
$('body').on('DOMSubtreeModified', '.chat-messages', function () {
    $('.chat-messages')[0].scrollTop = $('.chat-messages')[0].scrollHeight;
});

$('.chat-messages')[0].scrollTop = $('.chat-messages')[0].scrollHeight;