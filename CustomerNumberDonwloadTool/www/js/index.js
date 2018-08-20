 var form, $, table;
 var recordcardnumber = "";

 $(function () {

     //  $("#menu").metisMenu();

     $(".img-device").click(function () {
         layer.open({
             type: 1,
             area: '400px',
             title: "设置",
             closeBtn: 2,
             move: false,
             content: $("#setting"),
         });
     });

     $("#btnrefresh").click(function () {
         table.reload("datatable", {
             data: [],
         });
         var ret = RefreshClick();
         FuncBtnChange(ret);
     });

     $("#oldnumber").keypress(function (e) {
         return TxtKeypress(e);
     });

     $("#cardnumber").keypress(function (e) {
         return TxtKeypress(e);
     });

     function TxtKeypress(e) {
         var regex = /[a-fA-F0-9]/;
         var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
         if (!regex.test(key)) {
             return false;
         }
         return true;
     }

     $('.menu-item').click(function (data) {
         var elem = $(this);
         var active = $('.menu-item.active');
         if (elem != active) {
             active.removeClass('active');
             elem.addClass('active');
         }
     })

     $('.menu-item').mouseover(function (data) {
         $(this).find('.menu-item-tip').show();
     })

     $('.menu-item').mouseout(function (data) {
         $(this).find('.menu-item-tip').hide();
     })

     $("#navcardnumber").click(function () {
         $("#cardnumber_group").css("display", "block");
         $("#clientnumber_group").css("display", "none");
     });

     $("#navclientnumber").click(function () {
         $("#clientnumber_group").css("display", "block");
         $("#cardnumber_group").css("display", "none");
     });

 });

 $(document).keyup(function (event) {

     if (event.keyCode == 13 || event.keyCode == 32) { //enter
         var elem = $("#cardnumber_group:visible #btncardnumber:enabled");
         if (elem.length > 0) {
             $("#btncardnumber").trigger("click");
         }
     } else if (event.keyCode == 123) { //F12
         ShowDevTools();
     }
 });


 layui.use(['layer', 'table', 'form'], function () {
     form = layui.form;
     table = layui.table;
     var layer = layui.layer;

     form.verify({
         len: function (value) {
             if (value.length < 4) {
                 return "编号长度不正确（4 位数字）。";
             }
         },
         len6: function (value) {
             if (value.length < 6) {
                 return "卡片内码长度不正确（6 位数字）。";
             }
         }
     });


     form.on("checkbox(defaultoldnumber)", function (data) {
         var strDefault = "";
         if (data.elem.checked) {
             strDefault = "123456";
         }
         $("#oldnumber").val(strDefault);
         $("#oldnumber").attr("disabled", data.elem.checked);
     });

     form.on("submit(btndownload)", function () {
         var count = table.cache.datatable.length;
         if (count == 0) {
             NewsMessage("没有数据可以写入编号。");
             return false;
         }

         FuncBtnChange(true);
         var number = $("#clientnumber").val();
         var count = DownloadClick(number);
         if(count == 0){
            NewsMessage("没有可以下载编号的定距卡，请检测类型是否正常");
         }

         return false;
     });

     form.on("submit(btnclientnumber)", function () {
         FuncBtnChange(true);
         var number = $("#clientnumber").val();
         var ret = SetDeviceClient(number);
         if (!ret) {
             FuncBtnChange(false);

             NewsMessage("设置失败，请检测连接是否正常。");
         }

         return false;
     });

     form.on("submit(btncardnumber)", function () {
         var oldnumber = $("#oldnumber").val();
         var number = $("#cardnumber").val();
         var type = $("#cardtype").val();
         var ret = SetCardNumber(oldnumber, number, type);
         FuncBtnChange(ret);
         return false;
     });

     form.on("select(cardtype)", function (data) {
         var ret = data.value == "70";
         if (ret) {
             recordcardnumber = $("#cardnumber").val();
             $("#cardnumber").val("797979");
         } else {
             if (recordcardnumber.length > 0) {
                 $("#cardnumber").val(recordcardnumber);
             }
         }
         $("#cardnumber").attr("disabled", ret);
     });

     form.on("select(state)", function (data) {
         var ret = data.value == "0";
         ChangeConnectionState(ret);
         $("#btndevice").attr("disabled", ret);
         if (ret) {
             $("#btndevice").addClass("layui-btn-disabled");
             $("#select_port").attr("disabled", true);
         } else {
             $("#btndevice").removeClass("layui-btn-disabled");
             var btn = $("#btndevice.layui-btn-danger");
             if (btn.length == 0) {
                 $("#select_port").attr("disabled", false);
             }
         }
         form.render();
     });

     form.on("submit(btndevice)", function () {
         var port = $("#select_port").val();
         var ret = ChangeSerialConnection(port);
         DeviceBtnChange(ret);
         SerialMessage(ret, port);
         return false;
     });

 });

 function EndMessage(state) {
     var ret = Boolean(state);
     var number = $("#cardnumber").val();
     $("#txt").text(number);
     if (ret) {
         $(".txt-group i").removeClass("fa-close");
         $(".txt-group i").addClass("fa-check");
         $(".txt-group i").css("color", "green");
     } else {
         $(".txt-group i").removeClass("fa-check");
         $(".txt-group i").addClass("fa-close");
         $(".txt-group i").css("color", "red");
     }
     FuncBtnChange(false);
 }

 function InitConfig(number) {
     $("#cardnumber").val(number);

     IncrementingNumber();
 }

 function IncrementingNumber() {
     var type = $("#cardtype").val();
     if (type == "70") return;

     var strNumber = $("#cardnumber").val();
     var number = parseInt(strNumber, 16);
     number += 1;
     if (number == 1193046 || number == 7960953) { //1193046 = hex 123456 | 7960953 = hex 797979
         number += 1;
     }
     var hexNumber = number.toString(16);
     var ret = PrefixInterger(hexNumber, 6).toUpperCase();
     $("#cardnumber").val(ret);
 }

 function PrefixInterger(num, length) {
     return (Array(length).join('0') + num).slice(-length);
 }

 function DisplayErrorMessage(msg) {
     layer.open({
         closeBtn: 2,
         move: false,
         icon: 0,
         content: msg,
         btn: ['关闭'],
     });
 }

 function NewsMessage(msg) {
     layer.msg(msg, {
         offset: 'b',
         anim: 6,
         icon: 1,
     });
 }

 function PortChange(portnames) {
     var jsonObj = JSON.parse(portnames);
     var optionhtml = "";
     for (var i = 0; i < jsonObj.length; i++) {
         optionhtml += "<option value='" + jsonObj[i] + "'>" + jsonObj[i] + "</option>";
     }
     var elem = $("#select_port");
     elem.html(optionhtml);
     form.render();
     layer.msg("监测到端口设备发生变化。", {
         offset: "b",
         amin: 1,
         icon: 0,
     });
 }

 function SerialChange(serial) {
     var jsonObj = JSON.parse(serial);
     var open = jsonObj['IsOpen'];
     if (open) {
         var portName = jsonObj["PortName"];
         $("#select_port").val(portName);
     }
     DeviceBtnChange(open);
     SerialMessage(open, portName);
 }

 function SerialMessage(state, portName) {
     if (state) {
         layer.msg("设备连接成功，端口号：" + portName + "", {
             offset: 'b',
             anim: 6,
             icon: 1,
         });
     } else {
         layer.msg("设备已断开，端口号：" + portName + "", {
             offset: 'b',
             anim: 6,
             icon: 2,
         });
     }
 }

 function DeviceBtnChange(state) {
     FuncBtnChange(!state);
     var btn = $("#btndevice");
     if (state) {
         btn.text("关闭");
         btn.addClass("layui-btn-danger");
     } else {
         btn.text("打开");
         btn.removeClass("layui-btn-danger");
     }
     if ($("#select_state").val() == "1") {
         $("#select_port").attr("disabled", state);
     }
     form.render();
 }

 function FuncBtnChange(state) {
     state = Boolean(state);
     $("#btnrefresh").attr("disabled", state);
     $("#btndownload").attr("disabled", state);
     $("#btnclientnumber").attr("disabled", state);
     $("#btncardnumber").attr("disabled", state);
     $("#clientnumber").attr("disabled", state);
     $("#cardtype").attr("disabled", state);
     if ($("#cardtype").val() != "70") {
         $("#cardnumber").attr("disabled", state);
     }
     if (!state) {
         $("#btnrefresh").removeClass("layui-btn-disabled");
         $("#btndownload").removeClass("layui-btn-disabled");
         $("#btnclientnumber").removeClass("layui-btn-disabled");
         $("#btncardnumber").removeClass("layui-btn-disabled");
     } else {
         $("#btnrefresh").addClass("layui-btn-disabled");
         $("#btndownload").addClass("layui-btn-disabled");
         $("#btnclientnumber").addClass("layui-btn-disabled");
         $("#btncardnumber").addClass("layui-btn-disabled");
     }
 }

 function ConnectionFiall() {
     layer.confirm("未能连接设备，确认重新连接。", {
         btn: ['取消', '确认'],
         btn2: function () {
             ReconnectDevice();
         }
     });
 }

 function ListDisplay(strjson) {
     var jsonObj = JSON.parse(strjson);
     table.reload("datatable", {
         data: jsonObj,
     });
 }