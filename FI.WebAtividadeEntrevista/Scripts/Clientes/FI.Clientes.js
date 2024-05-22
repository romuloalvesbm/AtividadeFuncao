
$(document).ready(function () {
    $('#Cpf').mask('000.000.000-00', { reverse: true });
    $('#BeneficiarioCpf').mask('000.000.000-00', { reverse: true });
    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "Cpf": $(this).find("#Cpf").val(),
                "Beneficiarios": obterBeneficiarios(),
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON, false);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.", false);
                },
            success:
                function (r) {
                    if (typeof r.success !== 'undefined' && !r.success) {
                        ModalDialog("Ocorreu um erro", r.message, false);
                    }
                    else {
                        ModalDialog("Sucesso!", r, true);
                    }
                }
        });
    })

})

function ModalDialog(titulo, texto, close) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal" onclick="CloseModalDialog(' + close + ')">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}

function CloseModalDialog(close) {
    if (close) {
        $("#formCadastro")[0].reset();
    }
    else {
        $('.modalDlg').modal('hide').on('hidden.bs.modal', function () {
            $(this).remove(); // Remove o modal do DOM após ele ser fechado
        });
    }
}

$('#gridBeneficiarios').jtable({
    fields: {
        Cpf: {
            title: 'CPF',
            key: true,
            width: '25%',
        },
        Nome: {
            title: 'Nome',
            width: '60%',
        },
        Excluir: {
            title: 'Excluir',
            width: '15%',
            display: function (data) {
                return '<button class="btn btn-danger" onclick="excluirLinha(this)" title="Excluir">Excluir</button>';
            }
        }
    }
});

var dadosBeneficiarios = [];

$('#btnAddBeneficiario').click(function () {


    var nome = $('#BeneficiarioNome').val();
    var cpf = $('#BeneficiarioCpf').val().replace(/[.\-]/g, '');

    if (nome.trim() == '' || cpf.trim() === '') {
        ModalDialog("Alerta", "Favor preencher todos os campos.", false);
        return;
    }

    if (!TestaCPF(cpf)) {
        ModalDialog("Alerta", "CPF inválido.", false);
        return;
    }
    var cpfExistente = dadosBeneficiarios.some(function (beneficiario) {
        return beneficiario.Cpf === cpf;
    });

    if (cpfExistente) {
        ModalDialog("Alerta", "CPF do beneficiário já cadastrado.", false);
    }
    else {
        // Adicionar o novo beneficiário aos dados locais
        dadosBeneficiarios.push({
            Nome: nome,
            Cpf: cpf
        });

        // Adicionar o novo cliente à tabela
        $('#gridBeneficiarios').jtable('addRecord', {
            record: {
                Nome: nome,
                Cpf: $('#BeneficiarioCpf').val()
            },
            clientOnly: true,
            success: function () {
                // Limpar os campos após a adição bem-sucedida
                $('#BeneficiarioNome').val('');
                $('#BeneficiarioCpf').val('');
            },
            error: function () {
                alert('Erro ao adicionar Beneficiario.');
            }
        });
    }
});

function obterBeneficiarios() {
    return dadosBeneficiarios;
}

function excluirLinha(button) {
    var row = button.parentNode.parentNode;
    var cpf = row.cells[0].textContent.trim();

    var index = dadosBeneficiarios.findIndex(function (beneficiario) {
        return beneficiario.Cpf === cpf.replace(/[.\-]/g, '');
    });
    if (index !== -1) {
        dadosBeneficiarios.splice(index, 1);
    }

    $('#gridBeneficiarios').jtable('deleteRecord', {
        key: cpf,
        clientOnly: true,
        error: function () {
            alert('Erro ao excluir Beneficiario.');
        }
    });
};

function TestaCPF(cpf) {
    var Soma = 0
    var Resto

    var strCPF = String(cpf).replace(/[^\d]/g, '')

    if (strCPF.length !== 11)
        return false

    if ([
        '00000000000',
        '11111111111',
        '22222222222',
        '33333333333',
        '44444444444',
        '55555555555',
        '66666666666',
        '77777777777',
        '88888888888',
        '99999999999',
    ].indexOf(strCPF) !== -1)
        return false

    for (i = 1; i <= 9; i++)
        Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);

    Resto = (Soma * 10) % 11

    if ((Resto == 10) || (Resto == 11))
        Resto = 0

    if (Resto != parseInt(strCPF.substring(9, 10)))
        return false

    Soma = 0

    for (i = 1; i <= 10; i++)
        Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i)

    Resto = (Soma * 10) % 11

    if ((Resto == 10) || (Resto == 11))
        Resto = 0

    if (Resto != parseInt(strCPF.substring(10, 11)))
        return false

    return true
}