
var dadosBeneficiarios = [];
$(document).ready(function () {
    $('#Cpf').mask('000.000.000-00', { reverse: false });
    $('#BeneficiarioCpf').mask('000.000.000-00', { reverse: true });
    if (obj) {
        $('#formCadastro #Nome').val(obj.Nome);
        $('#formCadastro #CEP').val(obj.CEP);
        $('#formCadastro #Email').val(obj.Email);
        $('#formCadastro #Sobrenome').val(obj.Sobrenome);
        $('#formCadastro #Nacionalidade').val(obj.Nacionalidade);
        $('#formCadastro #Estado').val(obj.Estado);
        $('#formCadastro #Cidade').val(obj.Cidade);
        $('#formCadastro #Logradouro').val(obj.Logradouro);
        $('#formCadastro #Telefone').val(obj.Telefone);
        $('#formCadastro #Cpf').val(obj.Cpf);

        dadosBeneficiarios = JSON.parse(JSON.stringify(obj.Beneficiarios));

        $('#gridBeneficiarios').jtable({
            actions: {
                listAction: function (postData, jtParams) {
                    return {
                        Result: "OK",
                        Records: dadosBeneficiarios,
                        TotalRecordCount: dadosBeneficiarios.length
                    };
                }
            },
            fields: {
                id: {
                    key: true,
                    list: false
                },
                Cpf: {
                    title: 'Cpf',
                    width: '20%',
                    display: function (data) {
                        return formatarCPF(data.record.Cpf);
                    }
                },
                Nome: {
                    title: 'Nome',
                    width: '40%'
                }
            }
        });

    }

    $('#gridBeneficiarios').jtable('load');

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
                "Beneficiarios": dadosBeneficiarios
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
    var texto = '<div id="' + random + '" class="modal fade modalDlg">                                                               ' +
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
        '                <button type="button" class="btn btn-default" data-dismiss="modal" onclick="CloseModalDialog(' + close + ')">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}

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

function CloseModalDialog(close) {
    if (close) {
        $("#formCadastro")[0].reset();
        window.location.href = urlRetorno;
    }
    else {
        $('.modalDlg').modal('hide').on('hidden.bs.modal', function () {
            $(this).remove(); // Remove o modal do DOM após ele ser fechado
        });
    }
}

// Função para formatar CPF
function formatarCPF(cpf) {
    cpf = cpf.replace(/[^\d]/g, ''); // Remove caracteres não numéricos
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
}