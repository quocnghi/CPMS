app.controller("DcctController", function ($scope, DcctService, $window, $timeout) {
    getInfo();
    $scope.KKienThuc = [];
    $scope.CDRCTDT = [];
    $scope.cdrmonhocs = [];
    $scope.noidunggds = [];
    $scope.matrans = [];
    $scope.khdts = [];
    $scope.MAHT1 = [];
    $scope.tailieuthamkhaos = []

    function getInfo() {
        DcctService.getInfo().success(function (pro) {
            console.log(pro)
            angular.forEach(pro.cdrhp, function (value, key) {
                if (value.MaHT1.length > 0) {
                    var maELO = "";
                    angular.forEach(value.MaHT1, function (valuer, key) {
                        if (valuer.MaELO != null) {
                            maELO += valuer.MaELO + ';';
                            valuer.MaELO = maELO;
                        }

                    })
                } else {
                    value.MaHT1 = null;
                }

            })
            angular.forEach(pro.ndung, function (value, key) {
                if (value.Mota.length > 0) {
                    var maCELO = "";
                    angular.forEach(value.Mota, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maCELO += valuer.MaHT + ';';
                            valuer.MaHT = maCELO;
                        }

                    })
                } else {
                    value.Mota = null;
                }

            })
            var tenmhht = "";
            angular.forEach(pro.lstht, function (value, key) {
                if (value.TenMH != null) {                    
                    tenmhht += value.TenMH + '-';
                    value.TenMH = tenmhht
                } else {
                    value.TenMH = null;
                }

            })
            var tenmhtq = "";
            angular.forEach(pro.lsttq, function (value, key) {
                if (value.TenMH != null) {
                    tenmhtq += value.TenMH + '-';
                    value.TenMH = tenmhtq
                } else {
                    value.TenMH = null;
                }

            })
            $scope.listht = pro.lstht;
            $scope.listtq = pro.lsttq;
            $scope.khdt = pro.mhoc;
            $scope.LoaiPH = pro.loaiPH;
            $scope.KKienThuc = pro.KKT;
            $scope.CDRCTDT = pro.CDR;
            $scope.cdrmonhocs = pro.cdrhp;
            $scope.matrans = pro.mtcdr;
            $scope.noidunggds = pro.ndung;
            angular.forEach(pro.tHP, function (value, key) {
                if (value.NamXB != null) {
                    var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                    value.NamXB = date.getFullYear();
                }
            })
            if (pro.mhoc.Hinhthuc == 'BB') {
                $scope.checkBB = true;
            } else if (pro.mhoc.Hinhthuc == 'TC') {
                $scope.checkTC = true;
            }
            $scope.tailieuthamkhaos = pro.tHP;
            $scope.valueLoaiPH = []
            angular.forEach(pro.mhoc.loaiPH, function (val) {
                console.log(val)
                $scope.valueLoaiPH.push(val.MaLoaiPH);
            });
            console.log($scope.valueLoaiPH)
            var model = angular.element('#maloaiph');
            model.val($scope.valueLoaiPH).change();
            console.log(model)
        }).error(function (e) {
            alert('Có lỗi xảy ra' + e);
        });
    }

    $scope.changeTC = function () {
        $scope.BB = true;
    }

    $scope.changeBB = function () {
        $scope.TC = true;
    }

    $scope.addKQHTMD = function () {
        $scope.cdrMH = null;
        $scope.CDRMH = null;
    }

    // Adding CDRMH
    $scope.addCdr = function (cdrmh, matran) {
        var data = { CDRMH: cdrmh, MaELO: matran }
        console.log(data)
        DcctService.addCdr(data).success(function (msg) {
            console.log(msg)
            toastr.success(msg.msg);
            var model = angular.element('#addCdr');
            model.modal('hide');
            angular.forEach(msg.cdrhp, function (value, key) {
                if (value.MaHT1.length > 0) {
                    var maELO = "";
                    angular.forEach(value.MaHT1, function (valuer, key) {
                        if (valuer.MaELO != null) {
                            maELO += valuer.MaELO + ';';
                            valuer.MaELO = maELO;
                        }

                    })
                } else {
                    value.MaHT1 = null;
                }

            })
            $scope.cdrmonhocs = msg.cdrhp;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du lieu chuan dau ra ctdt
    $scope.getCdr = function (cdrhp) {
        DcctService.getCdr(cdrhp).success(function (msg) {         
            $scope.cdrMHedit = msg.cdrhp;
            console.log(msg.cdrhp)
            $scope.selectedValues = []
            $scope.dccdr = [];
            angular.forEach(msg.cdrhp.MaHT1, function (val) {
                console.log(val)
                $scope.selectedValues.push(val.MaELO);
            });
            console.log($scope.selectedValues)
            var model = angular.element('#maelo');
            model.val($scope.selectedValues).change();
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit chuan dau ra ctdt
    $scope.editCdr = function (Cdr, Matran) {
        var data = { CDRMH: Cdr, MaELO: Matran }
        console.log(data);
        DcctService.editCdr(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.cdrmonhocs = msg.cdrhp;
            var model = angular.element('#editCdr');
            model.modal('hide');
            angular.forEach(msg.cdrhp, function (value, key) {
                if (value.MaHT1.length > 0) {
                    var maELO = "";
                    angular.forEach(value.MaHT1, function (valuer, key) {
                        if (valuer.MaELO != null) {
                            maELO += valuer.MaELO + ';';
                            valuer.MaELO = maELO;
                        }

                    })
                } else {
                    value.MaHT1 = null;
                }

            })
        }, function () {
            alert('Error in adding record');
        });
    }
    // delete chuan dau ra
    $scope.deleteCdr = function (chp, index) {
        DcctService.deleteCdr(chp.MaCELO).success(function (msg) {
            toastr.success(msg.msg);
            $scope.cdrmonhocs.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

    $scope.addNDGD = function () {
        $scope.ndgd = null;
        $scope.NDGD = null;
    }

    // Adding NDGiangDay
    $scope.addND = function (NdGd, Cdr) {
        var data = { NDGD: NdGd, MaCELO: Cdr }
        console.log(data);
        DcctService.addND(data).success(function (msg) {
            toastr.success(msg.msg);
            angular.forEach(msg.ndGiangDay, function (value, key) {
                if (value.Mota.length > 0) {
                    var maCELO = "";
                    angular.forEach(value.Mota, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maCELO += valuer.MaHT + ';';
                            valuer.MaHT = maCELO;
                        }

                    })
                } else {
                    value.Mota = null;
                }
            })
            $scope.noidunggds = msg.ndGiangDay;
        }, function () {
            alert('Error in adding record');
        });
    }
    // lay du lieu noi dung giang day
    $scope.getND = function (ndHP) {
        DcctService.getND(ndHP).success(function (msg) {
            $scope.editndgd = msg.ndhp;
            $scope.selectedValuesND = []
            angular.forEach(msg.ndhp.Mota, function (val) {
                console.log(val)
                $scope.selectedValuesND.push(val.MaCELO);
            });
            console.log($scope.selectedValuesND)
            var model = angular.element('#macelo');
            model.val($scope.selectedValuesND).change();
            console.log(model)
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit noi dung giang day
    $scope.editND = function (ndung, cdr) {
        var data = { NDGD: ndung, MaCELO: cdr }
        console.log(data);
        DcctService.editND(data).success(function (msg) {
            toastr.success(msg.msg);
            angular.forEach(msg.ndunghp, function (value, key) {
                if (value.Mota.length > 0) {
                    var maCELO = "";
                    angular.forEach(value.Mota, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maCELO += valuer.MaHT + ';';
                            valuer.MaHT = maCELO;
                        }

                    })
                } else {
                    value.Mota = null;
                }
            })
            $scope.noidunggds = msg.ndunghp;
        }, function () {
            alert('Error in adding record');
        }
        );
    }

    // delete noi dung giang day
    $scope.deleteND = function (ng, index) {
        DcctService.deleteND(ng.MaND).success(function (msg) {
            toastr.success(msg.msg);
            $scope.noidunggds.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

    // review noi dung
    $scope.getreview = function (id) {
        DcctService.getReviewInfo(id).success(function (pro) {
            angular.forEach(pro.cdrhp, function (value, key) {
                if (value.MaHT1.length > 0) {
                    var maELO = "";
                    angular.forEach(value.MaHT1, function (valuer, key) {
                        if (valuer.MaELO != null) {
                            maELO += valuer.MaELO + ';';
                            valuer.MaELO = maELO;
                        }

                    })
                } else {
                    value.MaHT1 = null;
                }

            })
            angular.forEach(pro.ndhp, function (value, key) {
                if (value.Mota.length > 0) {
                    var maCELO = "";
                    angular.forEach(value.Mota, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maCELO += valuer.MaHT + ';';
                            valuer.MaHT = maCELO;
                        }

                    })
                } else {
                    value.Mota = null;
                }

            })

            $scope.TenHP = pro.mhoc.TenHP;
            $scope.MaHP = pro.mhoc.MaHP;
            $scope.TenCTDT = pro.mhoc.TenCTDT;
            $scope.SoTC = pro.mhoc.SoTC;
            $scope.GioLT = pro.mhoc.GioLT;
            $scope.GioTH = pro.mhoc.GioTH;
            $scope.GioDA = pro.mhoc.GioDA;
            $scope.GioTT = pro.mhoc.GioTT;
            $scope.Hocky = pro.mhoc.Hocky;
            $scope.NgonnguGD = pro.mhoc.NgonnguGD;
            $scope.Mota = pro.mhoc.Mota;
            $scope.MonTQ = pro.mhoc.MonTQ;
            $scope.MonHT = pro.mhoc.MonHT;
            $scope.NoidungCN = pro.mhoc.NoidungCN;
            $scope.MuctieuHP = pro.mhoc.MuctieuHP;
            $scope.MotaKienthuc = pro.mhoc.MotaKienthuc;
            $scope.PPGD = pro.mhoc.PPGD;
            $scope.PPHT = pro.mhoc.PPHT;
            $scope.NhiemvuSV = pro.mhoc.NhiemvuSV;
            $scope.TailieuHT = pro.mhoc.TailieuHT;
            $scope.Thangdiem = pro.mhoc.Thangdiem;
            $scope.PhuongtienGD = pro.mhoc.PhuongtienGD;
            $scope.PhuongtienThi = pro.mhoc.PhuongtienThi;
            $scope.NoidungCN = pro.mhoc.NoidungCN;
            $scope.KKienThuc = pro.KKT;
            $scope.Hinhthuc = pro.mhoc.Hinhthuc;
            $scope.CDRCTDT = pro.CDR;
            $scope.cdrmonhocs = pro.cdrhp;
            $scope.matrans = pro.mtcdr;
            $scope.noidunggds = pro.ndhp;
            $scope.LoaiTL = pro.tHP.LoaiTL;
            $scope.TenTL = pro.tHP.TenTL;
            $scope.Tacgia = pro.tHP.Tacgia;
            $scope.NhaXB = pro.tHP.NhaXB;
            $scope.NamXB = pro.tHP.NamXB;
            $scope.TenHT = pro.ndhp.TenHT;
            $scope.Noidung = pro.ndhp.Noidung;
            $scope.Phanloai = pro.ndhp.Phanloai;
            $scope.Mota = pro.ndhp.Mota;

           

        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.changeGT = function () {
        $scope.TLTK = null;
        $scope.TLHT = null;
        $scope.GTC = null;
    }

    $scope.changeTLTK = function () {
        $scope.GT = null;
        $scope.TLHT = null;
        $scope.GTC = null;
    }

    $scope.changeTLHT = function () {
        $scope.TLTK = null;
        $scope.GT = null;
        $scope.GTC = null;
    }

    $scope.changeGTC = function () {
        $scope.TLTK = null;
        $scope.TLHT = null;
        $scope.GT = null;
    }

    $scope.addTL = function () {
        $scope.tk = null;
    }
    
    // Adding tai lieu
    $scope.addTailieu = function (tk, gt, tltk, gtc, tlht) {
        console.log(gt, tltk, gtc, tlht);
        if (tltk == null && gtc == null && tlht == null) {
            tk.LoaiTL = '1';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                tk.TenTL = tk.Duongdan;
                console.log('test');
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }

        } else if (gtc == null && gt == null && tlht == null) {
            tk.LoaiTL = '2';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                tk.TenTL = tk.Duongdan;
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        } else if (gt == null && tltk == null && tlht == null) {
            tk.LoaiTL = '3';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                tk.TenTL = tk.Duongdan;
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        } else if (gt == null && gtc == null && tltk == null) {
            tk.LoaiTL = '4';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                tk.TenTL = tk.Duongdan;
                console.log(tk);
                DcctService.addTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        }

    }

    // lay du lieu tai lieu
    $scope.getTailieu = function (tlhp) {
        $scope.checkgt = false;
        $scope.checktlht = false;
        $scope.checkgtc = false;
        $scope.checktltk = false;
        DcctService.getTailieu(tlhp).success(function (msg) {
            console.log(tlhp)
            if (msg.tlhp.NamXB != null) {
                var date = new Date(parseInt(msg.tlhp.NamXB.replace('/Date(', '')));
                msg.tlhp.NamXB = date.getFullYear();
            }
            if (msg.tlhp.LoaiTL == '1') {
                $scope.checkgt = true;
            } else if (msg.tlhp.LoaiTL == '2') {
                $scope.checktltk = true;
            } else if (msg.tlhp.LoaiTL == '3') {
                $scope.checkgtc = true;
            } else if (msg.tlhp.LoaiTL == '4') {
                $scope.checktlht = true;
            }
            $scope.edittk = msg.tlhp;
            if (msg.tlhp.Kieunhap == '1') {
                $scope.showTL = true;
            } else if (msg.tlhp.Kieunhap == '2') {
                $scope.showTL = false;
            }
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit tai lieu
    $scope.editTailieu = function (tk, gt, tltk, gtc, tlht) {
        if (tltk == null && gtc == null && tlht == null) {
            tk.LoaiTL = '1';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }

        } else if (gtc == null && gt == null && tlht == null) {
            tk.LoaiTL = '2';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        } else if (gt == null && tltk == null && tlht == null) {
            tk.LoaiTL = '3';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        } else if (gt == null && gtc == null && tltk == null) {
            tk.LoaiTL = '4';
            if (tk.Duongdan == undefined) {
                tk.Kieunhap = '1';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            } else {
                tk.Kieunhap = '2';
                console.log(tk);
                DcctService.editTailieu(tk).success(function (msg) {
                    console.log(msg)
                    angular.forEach(msg.tlthamkhao, function (value, key) {
                        if (value.NamXB != null) {
                            var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                            value.NamXB = date.getFullYear();
                        }
                    })
                    toastr.success(msg.msg);
                    $scope.tailieuthamkhaos = msg.tlthamkhao;
                }, function () {
                    alert('Error in adding record');
                });
            }
        }
    }
    // delete tai lieu
    $scope.deleteTailieu = function (tlieu, index) {
        DcctService.deleteTailieu(tlieu.MaTL).success(function (msg) {
            toastr.success(msg.msg);
            $scope.tailieuthamkhaos.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

    $scope.hoanthanh = function (khdt, tc , bb, maph) {
        console.log(khdt);
        if (tc == true) {
            khdt.Hinhthuc = 'BB';
        }
        if (bb == true) {
            khdt.Hinhthuc = 'TC';
        }
        var data = { KHDT: khdt, MaPH: maph }
        console.log(data)
        DcctService.hoanthanh(data).success(function (ret) {
            toastr.success(ret.msg);
            $timeout(function () {
                $window.location.href = '/CMS/CourseSyllabus/CourseList/' + ret.id;
            }, 1500);
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.luunhap = function (khdt, tc , bb, maph) {
        console.log(tc, bb);
        if (tc == true) {
            khdt.Hinhthuc = 'BB';
        }
        if (bb == true) {
            khdt.Hinhthuc = 'TC';
        }
        var data = { KHDT: khdt, MaPH: maph }
        console.log(data.KHDT)
        DcctService.luunhap(data).success(function (ret) {
            toastr.success(ret.mg);
            $timeout(function () {
                $window.location.href = '/CMS/CourseSyllabus/CourseList/' + ret.id;
            }, 1500);
            
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }
});