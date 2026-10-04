namespace GrupoJap.Rentals.Localization;

public sealed record CatalogEntry(string Key, string Category, string? Description, string Pt, string En, string Es)
{
    public string For(string language) => language switch
    {
        "en" => En,
        "es" => Es,
        _ => Pt
    };
}

/// <summary>
/// Textos do painel com os valores iniciais em PT/EN/ES. Ao arrancar, as chaves em falta são inseridas na
/// base de dados (sem alterar o que já foi editado na administração); depois, estes valores só servem de recurso.
/// Para adicionar um texto novo: criar a entrada aqui e usar <c>@T["chave"]</c> na vista.
/// </summary>
public static class TranslationCatalog
{
    private const string Common = "Geral";
    private const string Layout = "Menu e layout";
    private const string Dashboard = "Painel";
    private const string Vehicles = "Veículos";
    private const string Customers = "Clientes";
    private const string Rentals = "Alugueres";
    private const string Translations = "Traduções";
    private const string Account = "Conta";
    private const string Profile = "Perfil";
    private const string Fields = "Campos de formulário";
    private const string Validation = "Validação";
    private const string Messages = "Mensagens";
    private const string Fuel = "Combustível";

    public static readonly IReadOnlyList<CatalogEntry> Entries =
    [
        // ---------- Geral ----------
        new("common.cancel", Common, null, "Cancelar", "Cancel", "Cancelar"),
        new("common.back_home", Common, null, "Voltar ao início", "Back to home", "Volver al inicio"),
        new("common.back", Common, null, "Voltar", "Back", "Volver"),
        new("common.edit", Common, "Ação nas linhas das tabelas", "Editar", "Edit", "Editar"),
        new("common.delete", Common, "Ação nas linhas das tabelas e botão de confirmação", "Eliminar", "Delete", "Eliminar"),
        new("common.save_changes", Common, null, "Guardar alterações", "Save changes", "Guardar cambios"),
        new("common.th_status", Common, "Cabeçalho de coluna", "ESTADO", "STATUS", "ESTADO"),
        new("status.active", Common, "Contrato em curso", "Em curso", "In progress", "En curso"),
        new("status.upcoming", Common, "Contrato que ainda não começou", "Agendado", "Scheduled", "Programado"),
        new("status.completed", Common, "Contrato terminado", "Concluído", "Completed", "Finalizado"),
        new("status.available", Common, "Veículo sem contrato hoje", "Disponível", "Available", "Disponible"),
        new("status.rented", Common, "Veículo com contrato hoje", "Alugado", "Rented", "Alquilado"),
        new("error.title", Common, "Página de erro", "Ocorreu um erro", "Something went wrong", "Se produjo un error"),
        new("error.text", Common, "Página de erro", "Não foi possível processar o teu pedido. Tenta novamente dentro de momentos.", "We couldn't process your request. Please try again in a moment.", "No hemos podido procesar tu solicitud. Inténtalo de nuevo en unos momentos."),
        new("error.request_id", Common, "Página de erro", "ID do pedido", "Request ID", "ID de la solicitud"),

        // ---------- Menu e layout ----------
        new("brand.label", Layout, "Texto do logótipo para leitores de ecrã", "GrupoJap Rentals, painel", "GrupoJap Rentals, dashboard", "GrupoJap Rentals, panel"),
        new("nav.label", Layout, null, "Navegação principal", "Main navigation", "Navegación principal"),
        new("nav.operation", Layout, "Título da secção do menu", "OPERAÇÃO", "OPERATIONS", "OPERACIÓN"),
        new("nav.system", Layout, "Título da secção do menu", "SISTEMA", "SYSTEM", "SISTEMA"),
        new("nav.dashboard", Layout, "Menu", "Painel", "Dashboard", "Panel"),
        new("nav.vehicles", Layout, "Menu e título da página", "Veículos", "Vehicles", "Vehículos"),
        new("nav.customers", Layout, "Menu e título da página", "Clientes", "Customers", "Clientes"),
        new("nav.rentals", Layout, "Menu e título da página", "Alugueres", "Rentals", "Alquileres"),
        new("nav.my_profile", Layout, null, "O meu perfil", "My profile", "Mi perfil"),
        new("nav.translations", Layout, "Menu e título da página", "Traduções", "Translations", "Traducciones"),
        new("nav.settings", Layout, null, "Definições", "Settings", "Ajustes"),
        new("nav.soon", Layout, "Etiqueta de funcionalidade futura", "BREVE", "SOON", "PRONTO"),
        new("nav.soon_hint", Layout, null, "Disponível numa próxima fase", "Available in a later phase", "Disponible en una fase posterior"),
        new("nav.open_navigation", Layout, "Botão do menu em ecrãs pequenos", "Abrir navegação", "Open navigation", "Abrir navegación"),
        new("nav.close_navigation", Layout, null, "Fechar navegação", "Close navigation", "Cerrar navegación"),
        new("nav.edit_profile", Layout, "Atalho na barra lateral", "Editar o meu perfil", "Edit my profile", "Editar mi perfil"),
        new("nav.logout", Layout, null, "Terminar sessão", "Sign out", "Cerrar sesión"),
        new("nav.login", Layout, null, "Entrar", "Sign in", "Iniciar sesión"),
        new("nav.language", Layout, "Seletor de idioma", "Idioma", "Language", "Idioma"),
        new("sidebar.db_status", Layout, "Estado da ligação à base de dados", "Ligação ativa", "Connection active", "Conexión activa"),
        new("theme.to_dark", Layout, "Botão do modo noturno", "Ativar modo noturno", "Enable dark mode", "Activar modo oscuro"),
        new("theme.to_light", Layout, "Botão do modo noturno", "Ativar modo claro", "Enable light mode", "Activar modo claro"),
        new("theme.toggle", Layout, "Dica do botão do modo noturno", "Alternar modo noturno", "Toggle dark mode", "Alternar modo oscuro"),
        new("topbar.fleet", Layout, "Início da data na barra superior", "FROTA", "FLEET", "FLOTA"),

        // ---------- Painel ----------
        new("dash.title", Dashboard, "Título do separador", "Visão geral", "Overview", "Resumen"),
        new("dash.heading", Dashboard, null, "Visão geral da frota", "Fleet overview", "Resumen de la flota"),
        new("dash.lead", Dashboard, null, "O essencial da operação, num só lugar.", "The essentials of your operation, in one place.", "Lo esencial de la operación, en un solo lugar."),
        new("dash.metrics_label", Dashboard, null, "Indicadores da frota", "Fleet indicators", "Indicadores de la flota"),
        new("dash.fleet_vehicles", Dashboard, null, "Veículos na frota", "Vehicles in the fleet", "Vehículos en la flota"),
        new("dash.registered_units", Dashboard, null, "Unidades registadas", "Registered units", "Unidades registradas"),
        new("dash.available_today", Dashboard, null, "Disponíveis hoje", "Available today", "Disponibles hoy"),
        new("dash.ready_to_rent", Dashboard, null, "Prontos para alugar", "Ready to rent", "Listos para alquilar"),
        new("dash.active_rentals", Dashboard, null, "Alugueres ativos", "Active rentals", "Alquileres activos"),
        new("dash.in_progress_today", Dashboard, null, "Em curso hoje", "In progress today", "En curso hoy"),
        new("dash.registered_in_system", Dashboard, null, "Registados no sistema", "Registered in the system", "Registrados en el sistema"),
        new("dash.overview_label", Dashboard, null, "Estado da frota e atividade", "Fleet status and activity", "Estado de la flota y actividad"),
        new("dash.current_state", Dashboard, null, "ESTADO ATUAL", "CURRENT STATUS", "ESTADO ACTUAL"),
        new("dash.availability", Dashboard, null, "Disponibilidade", "Availability", "Disponibilidad"),
        new("dash.availability_aria", Dashboard, "{0} = percentagem", "{0}% dos veículos disponíveis", "{0}% of vehicles available", "{0}% de los vehículos disponibles"),
        new("dash.available_caps", Dashboard, "Legenda do anel de disponibilidade", "DISPONÍVEL", "AVAILABLE", "DISPONIBLE"),
        new("dash.legend_available", Dashboard, null, "Disponíveis", "Available", "Disponibles"),
        new("dash.legend_rented", Dashboard, null, "Alugados", "Rented", "Alquilados"),
        new("dash.total_fleet", Dashboard, null, "Total da frota", "Total fleet", "Total de la flota"),
        new("dash.state_caption", Dashboard, null, "Estado calculado com base nos contratos de hoje", "Status calculated from today's contracts", "Estado calculado según los contratos de hoy"),
        new("dash.movement", Dashboard, null, "MOVIMENTO", "ACTIVITY", "MOVIMIENTO"),
        new("dash.recent_contracts", Dashboard, null, "Contratos recentes", "Recent contracts", "Contratos recientes"),
        new("dash.search_contracts", Dashboard, "Texto para leitores de ecrã", "Pesquisar contratos", "Search contracts", "Buscar contratos"),
        new("dash.search", Dashboard, "Texto de ajuda do campo de pesquisa", "Pesquisar", "Search", "Buscar"),
        new("dash.th_customer_vehicle", Dashboard, "Cabeçalho de coluna", "CLIENTE / VEÍCULO", "CUSTOMER / VEHICLE", "CLIENTE / VEHÍCULO"),
        new("dash.th_period", Dashboard, "Cabeçalho de coluna", "PERÍODO", "PERIOD", "PERIODO"),
        new("dash.empty_title", Dashboard, null, "Sem contratos registados", "No contracts registered", "Sin contratos registrados"),
        new("dash.empty_text", Dashboard, null, "Os novos alugueres vão aparecer aqui.", "New rentals will appear here.", "Los nuevos alquileres aparecerán aquí."),
        new("dash.no_search_results", Dashboard, null, "Nenhum contrato corresponde à pesquisa.", "No contract matches your search.", "Ningún contrato coincide con la búsqueda."),
        new("dash.showing", Dashboard, "{0} = mostrados, {1} = total", "A mostrar {0} de {1} contratos", "Showing {0} of {1} contracts", "Mostrando {0} de {1} contratos"),
        new("dash.recent_activity", Dashboard, null, "ATIVIDADE RECENTE", "RECENT ACTIVITY", "ACTIVIDAD RECIENTE"),
        new("dash.footer_management", Dashboard, "Rodapé do painel", "GESTÃO DE FROTA", "FLEET MANAGEMENT", "GESTIÓN DE FLOTA"),

        // ---------- Veículos ----------
        new("vehicles.eyebrow", Vehicles, "Separa as partes com \" · \"", "OPERAÇÃO · FROTA", "OPERATIONS · FLEET", "OPERACIÓN · FLOTA"),
        new("vehicles.eyebrow_new", Vehicles, null, "FROTA · NOVO REGISTO", "FLEET · NEW ENTRY", "FLOTA · NUEVO REGISTRO"),
        new("vehicles.eyebrow_edit", Vehicles, null, "FROTA · EDIÇÃO", "FLEET · EDIT", "FLOTA · EDICIÓN"),
        new("vehicles.eyebrow_delete", Vehicles, null, "FROTA · ELIMINAR", "FLEET · DELETE", "FLOTA · ELIMINAR"),
        new("vehicles.count", Vehicles, "{0} = número de veículos", "{0} veículo(s) registado(s)", "{0} vehicle(s) registered", "{0} vehículo(s) registrado(s)"),
        new("vehicles.add", Vehicles, "Botão e título do formulário", "Registar veículo", "Add vehicle", "Registrar vehículo"),
        new("vehicles.list_label", Vehicles, null, "Lista de veículos", "Vehicle list", "Lista de vehículos"),
        new("vehicles.th_vehicle", Vehicles, null, "VEÍCULO", "VEHICLE", "VEHÍCULO"),
        new("vehicles.th_registration", Vehicles, null, "MATRÍCULA", "REGISTRATION", "MATRÍCULA"),
        new("vehicles.th_year", Vehicles, null, "ANO", "YEAR", "AÑO"),
        new("vehicles.th_fuel", Vehicles, null, "COMBUSTÍVEL", "FUEL", "COMBUSTIBLE"),
        new("vehicles.empty_title", Vehicles, null, "A frota ainda não tem veículos", "The fleet has no vehicles yet", "La flota aún no tiene vehículos"),
        new("vehicles.empty_text", Vehicles, null, "Regista o primeiro veículo para começares a gerir disponibilidade e alugueres.", "Add the first vehicle to start managing availability and rentals.", "Registra el primer vehículo para empezar a gestionar la disponibilidad y los alquileres."),
        new("vehicles.create_lead", Vehicles, null, "Preenche os dados de identificação do veículo.", "Fill in the vehicle's identification details.", "Completa los datos de identificación del vehículo."),
        new("vehicles.back", Vehicles, null, "Voltar à frota", "Back to fleet", "Volver a la flota"),
        new("vehicles.save", Vehicles, null, "Guardar veículo", "Save vehicle", "Guardar vehículo"),
        new("vehicles.edit_title", Vehicles, null, "Editar veículo", "Edit vehicle", "Editar vehículo"),
        new("vehicles.delete_title", Vehicles, null, "Eliminar veículo", "Delete vehicle", "Eliminar vehículo"),
        new("vehicles.delete_blocked", Vehicles, "{0} = número de contratos", "Este veículo tem {0} contrato(s) associado(s) e não pode ser eliminado.", "This vehicle has {0} associated contract(s) and can't be deleted.", "Este vehículo tiene {0} contrato(s) asociado(s) y no se puede eliminar."),
        new("vehicles.delete_confirm", Vehicles, null, "Tens a certeza que queres eliminar este veículo? Esta ação não pode ser revertida.", "Are you sure you want to delete this vehicle? This action cannot be undone.", "¿Seguro que quieres eliminar este vehículo? Esta acción no se puede deshacer."),
        new("vehicles.placeholder_model", Vehicles, null, "Ex.: Corolla", "e.g. Corolla", "Ej.: Corolla"),
        new("vehicles.placeholder_registration", Vehicles, null, "Ex.: AA-00-BB", "e.g. AA-00-BB", "Ej.: AA-00-BB"),

        // ---------- Clientes ----------
        new("customers.eyebrow", Customers, "Separa as partes com \" · \"", "OPERAÇÃO · CLIENTES", "OPERATIONS · CUSTOMERS", "OPERACIÓN · CLIENTES"),
        new("customers.eyebrow_new", Customers, null, "CLIENTES · NOVO REGISTO", "CUSTOMERS · NEW ENTRY", "CLIENTES · NUEVO REGISTRO"),
        new("customers.eyebrow_edit", Customers, null, "CLIENTES · EDIÇÃO", "CUSTOMERS · EDIT", "CLIENTES · EDICIÓN"),
        new("customers.eyebrow_delete", Customers, null, "CLIENTES · ELIMINAR", "CUSTOMERS · DELETE", "CLIENTES · ELIMINAR"),
        new("customers.count", Customers, "{0} = número de clientes", "{0} cliente(s) registado(s)", "{0} customer(s) registered", "{0} cliente(s) registrado(s)"),
        new("customers.add", Customers, "Botão e título do formulário", "Registar cliente", "Add customer", "Registrar cliente"),
        new("customers.list_label", Customers, null, "Lista de clientes", "Customer list", "Lista de clientes"),
        new("customers.th_customer", Customers, null, "CLIENTE", "CUSTOMER", "CLIENTE"),
        new("customers.th_phone", Customers, null, "TELEFONE", "PHONE", "TELÉFONO"),
        new("customers.th_license", Customers, null, "CARTA DE CONDUÇÃO", "DRIVING LICENCE", "PERMISO DE CONDUCIR"),
        new("customers.th_contracts", Customers, null, "CONTRATOS", "CONTRACTS", "CONTRATOS"),
        new("customers.empty_title", Customers, null, "Ainda não há clientes", "There are no customers yet", "Aún no hay clientes"),
        new("customers.empty_text", Customers, null, "Regista o primeiro cliente para criares contratos de aluguer.", "Add the first customer to create rental contracts.", "Registra el primer cliente para crear contratos de alquiler."),
        new("customers.status_active", Customers, null, "Com aluguer ativo", "With active rental", "Con alquiler activo"),
        new("customers.status_none", Customers, null, "Sem aluguer ativo", "No active rental", "Sin alquiler activo"),
        new("customers.create_lead", Customers, null, "Preenche os dados de contacto e a carta de condução.", "Fill in the contact details and driving licence.", "Completa los datos de contacto y el permiso de conducir."),
        new("customers.back", Customers, null, "Voltar aos clientes", "Back to customers", "Volver a los clientes"),
        new("customers.save", Customers, null, "Guardar cliente", "Save customer", "Guardar cliente"),
        new("customers.edit_title", Customers, null, "Editar cliente", "Edit customer", "Editar cliente"),
        new("customers.delete_title", Customers, null, "Eliminar cliente", "Delete customer", "Eliminar cliente"),
        new("customers.delete_blocked", Customers, "{0} = número de contratos", "Este cliente tem {0} contrato(s) associado(s) e não pode ser eliminado.", "This customer has {0} associated contract(s) and can't be deleted.", "Este cliente tiene {0} contrato(s) asociado(s) y no se puede eliminar."),
        new("customers.delete_confirm", Customers, null, "Tens a certeza que queres eliminar este cliente? Esta ação não pode ser revertida.", "Are you sure you want to delete this customer? This action cannot be undone.", "¿Seguro que quieres eliminar este cliente? Esta acción no se puede deshacer."),

        // ---------- Alugueres ----------
        new("rentals.eyebrow", Rentals, "Separa as partes com \" · \"", "OPERAÇÃO · ALUGUERES", "OPERATIONS · RENTALS", "OPERACIÓN · ALQUILERES"),
        new("rentals.eyebrow_new", Rentals, null, "ALUGUERES · NOVO CONTRATO", "RENTALS · NEW CONTRACT", "ALQUILERES · NUEVO CONTRATO"),
        new("rentals.eyebrow_edit", Rentals, null, "ALUGUERES · EDIÇÃO", "RENTALS · EDIT", "ALQUILERES · EDICIÓN"),
        new("rentals.eyebrow_delete", Rentals, null, "ALUGUERES · ELIMINAR", "RENTALS · DELETE", "ALQUILERES · ELIMINAR"),
        new("rentals.heading", Rentals, null, "Contratos de aluguer", "Rental contracts", "Contratos de alquiler"),
        new("rentals.count", Rentals, "{0} = número de contratos", "{0} contrato(s) registado(s)", "{0} contract(s) registered", "{0} contrato(s) registrado(s)"),
        new("rentals.add", Rentals, "Botão e título da página", "Novo contrato", "New contract", "Nuevo contrato"),
        new("rentals.list_label", Rentals, null, "Lista de contratos", "Contract list", "Lista de contratos"),
        new("rentals.th_customer", Rentals, null, "CLIENTE", "CUSTOMER", "CLIENTE"),
        new("rentals.th_vehicle", Rentals, null, "VEÍCULO", "VEHICLE", "VEHÍCULO"),
        new("rentals.th_period", Rentals, null, "PERÍODO", "PERIOD", "PERIODO"),
        new("rentals.th_initial_km", Rentals, null, "KM INICIAL", "INITIAL KM", "KM INICIAL"),
        new("rentals.empty_title", Rentals, null, "Ainda não há contratos", "There are no contracts yet", "Aún no hay contratos"),
        new("rentals.empty_text", Rentals, null, "Cria o primeiro contrato associando um cliente a um veículo.", "Create the first contract by linking a customer to a vehicle.", "Crea el primer contrato asociando un cliente a un vehículo."),
        new("rentals.create_title", Rentals, null, "Novo contrato de aluguer", "New rental contract", "Nuevo contrato de alquiler"),
        new("rentals.create_lead", Rentals, null, "Associa um cliente a um veículo e define o período do aluguer.", "Link a customer to a vehicle and set the rental period.", "Asocia un cliente a un vehículo y define el periodo del alquiler."),
        new("rentals.back", Rentals, null, "Voltar aos alugueres", "Back to rentals", "Volver a los alquileres"),
        new("rentals.save", Rentals, null, "Guardar contrato", "Save contract", "Guardar contrato"),
        new("rentals.edit_title", Rentals, null, "Editar contrato", "Edit contract", "Editar contrato"),
        new("rentals.edit_lead", Rentals, "{0} = número do contrato", "Contrato #{0}", "Contract #{0}", "Contrato #{0}"),
        new("rentals.delete_title", Rentals, null, "Eliminar contrato", "Delete contract", "Eliminar contrato"),
        new("rentals.delete_confirm", Rentals, null, "Tens a certeza que queres eliminar este contrato? Esta ação não pode ser revertida.", "Are you sure you want to delete this contract? This action cannot be undone.", "¿Seguro que quieres eliminar este contrato? Esta acción no se puede deshacer."),
        new("rentals.select_customer", Rentals, "Primeira opção da lista", "Seleciona um cliente", "Select a customer", "Selecciona un cliente"),
        new("rentals.select_vehicle", Rentals, "Primeira opção da lista", "Seleciona um veículo", "Select a vehicle", "Selecciona un vehículo"),

        // ---------- Traduções ----------
        new("tr.eyebrow", Translations, "Separa as partes com \" · \"", "SISTEMA · TRADUÇÕES", "SYSTEM · TRANSLATIONS", "SISTEMA · TRADUCCIONES"),
        new("tr.eyebrow_new", Translations, null, "TRADUÇÕES · NOVA", "TRANSLATIONS · NEW", "TRADUCCIONES · NUEVA"),
        new("tr.eyebrow_edit", Translations, null, "TRADUÇÕES · EDIÇÃO", "TRANSLATIONS · EDIT", "TRADUCCIONES · EDICIÓN"),
        new("tr.eyebrow_delete", Translations, null, "TRADUÇÕES · ELIMINAR", "TRANSLATIONS · DELETE", "TRADUCCIONES · ELIMINAR"),
        new("tr.heading", Translations, null, "Traduções do painel", "Dashboard translations", "Traducciones del panel"),
        new("tr.count", Translations, "{0} = número de textos", "{0} texto(s) · edita aqui o que aparece no painel em português, inglês e espanhol.", "{0} text(s) · edit here what the dashboard shows in Portuguese, English and Spanish.", "{0} texto(s) · edita aquí lo que muestra el panel en portugués, inglés y español."),
        new("tr.add", Translations, "Botão e título do formulário", "Nova tradução", "New translation", "Nueva traducción"),
        new("tr.search", Translations, null, "Pesquisar", "Search", "Buscar"),
        new("tr.search_placeholder", Translations, null, "Chave, texto ou descrição", "Key, text or description", "Clave, texto o descripción"),
        new("tr.all_categories", Translations, "Opção \"todas\" do filtro de categoria", "Todas", "All", "Todas"),
        new("tr.only_incomplete", Translations, null, "Só incompletas", "Incomplete only", "Solo incompletas"),
        new("tr.filter", Translations, null, "Filtrar", "Filter", "Filtrar"),
        new("tr.clear", Translations, null, "Limpar", "Clear", "Limpiar"),
        new("tr.list_label", Translations, null, "Lista de traduções", "Translation list", "Lista de traducciones"),
        new("tr.th_key", Translations, null, "CHAVE", "KEY", "CLAVE"),
        new("tr.missing", Translations, "Idioma sem texto", "Em falta", "Missing", "Falta"),
        new("tr.empty_title", Translations, null, "Nenhuma tradução encontrada", "No translations found", "No se encontró ninguna traducción"),
        new("tr.empty_text", Translations, null, "Altera a pesquisa ou limpa os filtros.", "Change your search or clear the filters.", "Cambia la búsqueda o limpia los filtros."),
        new("tr.pagination", Translations, null, "Paginação", "Pagination", "Paginación"),
        new("tr.page_of", Translations, "{0} = página, {1} = total", "Página {0} de {1}", "Page {0} of {1}", "Página {0} de {1}"),
        new("tr.prev", Translations, null, "← Anterior", "← Previous", "← Anterior"),
        new("tr.next", Translations, null, "Seguinte →", "Next →", "Siguiente →"),
        new("tr.edit_title", Translations, null, "Editar tradução", "Edit translation", "Editar traducción"),
        new("tr.delete_title", Translations, null, "Eliminar tradução", "Delete translation", "Eliminar traducción"),
        new("tr.create_hint", Translations, null, "Depois de criada, usa @T[\"chave\"] numa vista para mostrar o texto.", "Once created, use @T[\"key\"] in a view to show the text.", "Una vez creada, usa @T[\"clave\"] en una vista para mostrar el texto."),
        new("tr.back", Translations, null, "Voltar às traduções", "Back to translations", "Volver a las traducciones"),
        new("tr.key_readonly", Translations, null, "A chave é usada no código das vistas e não pode ser alterada.", "The key is used in the views' code and can't be changed.", "La clave se usa en el código de las vistas y no se puede cambiar."),
        new("tr.key_placeholder", Translations, null, "Ex.: dash.promo.title", "e.g. dash.promo.title", "Ej.: dash.promo.title"),
        new("tr.category_placeholder", Translations, null, "Ex.: Painel", "e.g. Dashboard", "Ej.: Panel"),
        new("tr.description_placeholder", Translations, null, "Ex.: Título da secção de destaques", "e.g. Title of the highlights section", "Ej.: Título de la sección de destacados"),
        new("tr.required", Translations, "Junto ao nome do idioma", "(obrigatório)", "(required)", "(obligatorio)"),
        new("tr.original", Translations, null, "Original:", "Original:", "Original:"),
        new("tr.reset", Translations, "Repõe o texto original no campo", "Repor", "Reset", "Restablecer"),
        new("tr.empty_hint", Translations, null, "Se ficar vazio, o painel mostra o texto em português.", "If left empty, the dashboard shows the Portuguese text.", "Si queda vacío, el panel muestra el texto en portugués."),
        new("tr.create", Translations, null, "Criar tradução", "Create translation", "Crear traducción"),
        new("tr.key_label", Translations, null, "Chave", "Key", "Clave"),
        new("tr.delete_catalog_notice", Translations, null, "Este texto é usado no painel. Ao eliminar, o painel volta a mostrar o texto original e a tradução é recriada com os valores originais no próximo arranque. Na prática, funciona como \"repor\".", "This text is used in the dashboard. When deleted, the dashboard goes back to showing the original text and the translation is recreated with its original values on the next start. In practice, it works as a \"reset\".", "Este texto se usa en el panel. Al eliminarlo, el panel vuelve a mostrar el texto original y la traducción se recrea con sus valores originales en el próximo arranque. En la práctica, funciona como \"restablecer\"."),
        new("tr.delete_confirm", Translations, null, "Tens a certeza que queres eliminar esta tradução? Se ainda estiver a ser usada numa vista, o painel passa a mostrar a chave.", "Are you sure you want to delete this translation? If a view still uses it, the dashboard will show the key.", "¿Seguro que quieres eliminar esta traducción? Si una vista todavía la usa, el panel mostrará la clave."),

        // ---------- Mensagens ----------
        new("msg.vehicle_created", Messages, null, "Veículo registado com sucesso.", "Vehicle registered successfully.", "Vehículo registrado correctamente."),
        new("msg.vehicle_updated", Messages, null, "Veículo atualizado com sucesso.", "Vehicle updated successfully.", "Vehículo actualizado correctamente."),
        new("msg.vehicle_deleted", Messages, null, "Veículo eliminado com sucesso.", "Vehicle deleted successfully.", "Vehículo eliminado correctamente."),
        new("msg.vehicle_delete_blocked", Messages, null, "Não é possível eliminar um veículo com contratos de aluguer associados.", "A vehicle with associated rental contracts can't be deleted.", "No se puede eliminar un vehículo con contratos de alquiler asociados."),
        new("msg.customer_created", Messages, null, "Cliente registado com sucesso.", "Customer registered successfully.", "Cliente registrado correctamente."),
        new("msg.customer_updated", Messages, null, "Cliente atualizado com sucesso.", "Customer updated successfully.", "Cliente actualizado correctamente."),
        new("msg.customer_deleted", Messages, null, "Cliente eliminado com sucesso.", "Customer deleted successfully.", "Cliente eliminado correctamente."),
        new("msg.customer_delete_blocked", Messages, null, "Não é possível eliminar um cliente com contratos de aluguer associados.", "A customer with associated rental contracts can't be deleted.", "No se puede eliminar un cliente con contratos de alquiler asociados."),
        new("msg.rental_created", Messages, null, "Contrato de aluguer registado com sucesso.", "Rental contract registered successfully.", "Contrato de alquiler registrado correctamente."),
        new("msg.rental_updated", Messages, null, "Contrato de aluguer atualizado com sucesso.", "Rental contract updated successfully.", "Contrato de alquiler actualizado correctamente."),
        new("msg.rental_deleted", Messages, null, "Contrato de aluguer eliminado com sucesso.", "Rental contract deleted successfully.", "Contrato de alquiler eliminado correctamente."),
        new("msg.translation_created", Messages, "{0} = chave", "Tradução \"{0}\" criada. Usa @T[\"{0}\"] numa vista para a mostrar.", "Translation \"{0}\" created. Use @T[\"{0}\"] in a view to show it.", "Traducción \"{0}\" creada. Usa @T[\"{0}\"] en una vista para mostrarla."),
        new("msg.translation_updated", Messages, "{0} = chave", "Tradução \"{0}\" atualizada.", "Translation \"{0}\" updated.", "Traducción \"{0}\" actualizada."),
        new("msg.translation_deleted", Messages, "{0} = chave", "Tradução \"{0}\" eliminada.", "Translation \"{0}\" deleted.", "Traducción \"{0}\" eliminada."),
        new("msg.translation_reset", Messages, "{0} = chave", "Tradução \"{0}\" eliminada. O painel volta a usar o texto original, que será reposto na base de dados no próximo arranque.", "Translation \"{0}\" deleted. The dashboard goes back to the original text, which will be restored in the database on the next start.", "Traducción \"{0}\" eliminada. El panel vuelve a usar el texto original, que se repondrá en la base de datos en el próximo arranque."),

        // ---------- Conta ----------
        new("account.login.eyebrow", Account, null, "ACESSO", "ACCESS", "ACCESO"),
        new("account.login.lead", Account, null, "Inicia sessão para continuares.", "Sign in to continue.", "Inicia sesión para continuar."),
        new("account.denied.title", Account, null, "Acesso negado", "Access denied", "Acceso denegado"),
        new("account.denied.heading", Account, null, "Sem permissão", "No permission", "Sin permiso"),
        new("account.denied.lead", Account, null, "Esta área é reservada a administradores.", "This area is restricted to administrators.", "Esta área está reservada a administradores."),
        new("account.error.invalid", Account, null, "Email ou palavra-passe incorretos.", "Incorrect email or password.", "Correo o contraseña incorrectos."),
        new("account.error.locked", Account, null, "Conta temporariamente bloqueada por demasiadas tentativas. Tenta novamente dentro de alguns minutos.", "Account temporarily locked after too many attempts. Try again in a few minutes.", "Cuenta bloqueada temporalmente por demasiados intentos. Inténtalo de nuevo en unos minutos."),
        new("account.error.invalid_email", Account, null, "Indica um endereço de email válido.", "Enter a valid email address.", "Indica una dirección de correo válida."),
        new("password.error.too_short", Account, null, "A palavra-passe deve ter pelo menos 8 caracteres.", "The password must be at least 8 characters long.", "La contraseña debe tener al menos 8 caracteres."),
        new("password.error.digit", Account, null, "A palavra-passe deve conter pelo menos um número.", "The password must contain at least one number.", "La contraseña debe contener al menos un número."),
        new("password.error.upper", Account, null, "A palavra-passe deve conter pelo menos uma letra maiúscula.", "The password must contain at least one uppercase letter.", "La contraseña debe contener al menos una letra mayúscula."),
        new("password.error.lower", Account, null, "A palavra-passe deve conter pelo menos uma letra minúscula.", "The password must contain at least one lowercase letter.", "La contraseña debe contener al menos una letra minúscula."),
        new("password.error.symbol", Account, null, "A palavra-passe deve conter pelo menos um símbolo.", "The password must contain at least one symbol.", "La contraseña debe contener al menos un símbolo."),
        new("password.error.current_wrong", Account, null, "A palavra-passe atual está incorreta.", "The current password is incorrect.", "La contraseña actual es incorrecta."),

        // ---------- Perfil ----------
        new("profile.eyebrow", Profile, null, "CONTA · PERFIL", "ACCOUNT · PROFILE", "CUENTA · PERFIL"),
        new("profile.lead", Profile, null, "Atualiza os teus dados e a tua foto.", "Update your details and photo.", "Actualiza tus datos y tu foto."),
        new("profile.change_password", Profile, null, "Alterar palavra-passe", "Change password", "Cambiar contraseña"),
        new("profile.photo_alt", Profile, null, "Foto de perfil", "Profile photo", "Foto de perfil"),
        new("profile.photo_preview_alt", Profile, "Texto alternativo da pré-visualização", "Pré-visualização da foto", "Photo preview", "Vista previa de la foto"),
        new("profile.photo_hint", Profile, null, "JPG, PNG ou WebP, até 2 MB.", "JPG, PNG or WebP, up to 2 MB.", "JPG, PNG o WebP, hasta 2 MB."),
        new("profile.photo_empty", Profile, null, "O ficheiro da foto está vazio.", "The photo file is empty.", "El archivo de la foto está vacío."),
        new("profile.photo_too_large", Profile, null, "A foto não pode ter mais de 2 MB.", "The photo can't be larger than 2 MB.", "La foto no puede superar los 2 MB."),
        new("profile.photo_invalid", Profile, null, "Formato inválido. Usa uma imagem JPG, PNG ou WebP.", "Invalid format. Use a JPG, PNG or WebP image.", "Formato no válido. Usa una imagen JPG, PNG o WebP."),
        new("profile.role", Profile, null, "Perfil", "Role", "Rol"),
        new("profile.role_admin", Profile, null, "Administrador", "Administrator", "Administrador"),
        new("profile.role_user", Profile, null, "Utilizador", "User", "Usuario"),
        new("profile.job_placeholder", Profile, null, "Ex.: Gestor de frota", "e.g. Fleet manager", "Ej.: Gestor de flota"),
        new("profile.brand_placeholder", Profile, null, "Ex.: Toyota", "e.g. Toyota", "Ej.: Toyota"),
        new("profile.bio_placeholder", Profile, null, "Fala um pouco sobre ti.", "Tell us a little about yourself.", "Cuéntanos un poco sobre ti."),
        new("profile.save", Profile, null, "Guardar perfil", "Save profile", "Guardar perfil"),
        new("profile.saved", Profile, null, "Perfil atualizado com sucesso.", "Profile updated successfully.", "Perfil actualizado correctamente."),
        new("password.eyebrow", Profile, null, "CONTA · SEGURANÇA", "ACCOUNT · SECURITY", "CUENTA · SEGURIDAD"),
        new("password.lead", Profile, null, "Mínimo de 8 caracteres, com maiúscula, minúscula, número e símbolo.", "At least 8 characters, with an uppercase letter, a lowercase letter, a number and a symbol.", "Mínimo 8 caracteres, con mayúscula, minúscula, número y símbolo."),
        new("password.back", Profile, null, "Voltar ao perfil", "Back to profile", "Volver al perfil"),
        new("password.changed", Profile, null, "Palavra-passe alterada com sucesso.", "Password changed successfully.", "Contraseña cambiada correctamente."),

        // ---------- Campos de formulário ----------
        new("field.email", Fields, null, "Email", "Email", "Correo electrónico"),
        new("field.password", Fields, null, "Palavra-passe", "Password", "Contraseña"),
        new("field.remember", Fields, null, "Manter sessão iniciada", "Keep me signed in", "Mantener la sesión iniciada"),
        new("field.full_name", Fields, null, "Nome completo", "Full name", "Nombre completo"),
        new("field.phone", Fields, null, "Telefone", "Phone", "Teléfono"),
        new("field.phone_placeholder", Fields, null, "9 dígitos", "9 digits", "9 dígitos"),
        new("field.job_title", Fields, null, "Cargo", "Job title", "Cargo"),
        new("field.favorite_brand", Fields, null, "Marca favorita", "Favourite brand", "Marca favorita"),
        new("field.bio", Fields, null, "Sobre mim", "About me", "Sobre mí"),
        new("field.photo", Fields, null, "Nova foto", "New photo", "Nueva foto"),
        new("field.remove_photo", Fields, null, "Remover foto atual", "Remove current photo", "Eliminar foto actual"),
        new("field.current_password", Fields, null, "Palavra-passe atual", "Current password", "Contraseña actual"),
        new("field.new_password", Fields, null, "Nova palavra-passe", "New password", "Nueva contraseña"),
        new("field.confirm_new_password", Fields, null, "Confirmar nova palavra-passe", "Confirm new password", "Confirmar nueva contraseña"),
        new("field.driving_license", Fields, null, "Carta de condução", "Driving licence", "Permiso de conducir"),
        new("field.brand", Fields, null, "Marca", "Brand", "Marca"),
        new("field.model", Fields, null, "Modelo", "Model", "Modelo"),
        new("field.registration", Fields, null, "Matrícula", "Registration", "Matrícula"),
        new("field.year", Fields, null, "Ano de fabrico", "Year of manufacture", "Año de fabricación"),
        new("field.fuel_type", Fields, null, "Tipo de combustível", "Fuel type", "Tipo de combustible"),
        new("field.mileage", Fields, null, "Quilometragem atual", "Current mileage", "Kilometraje actual"),
        new("field.customer", Fields, null, "Cliente", "Customer", "Cliente"),
        new("field.vehicle", Fields, null, "Veículo", "Vehicle", "Vehículo"),
        new("field.start_date", Fields, null, "Data de início", "Start date", "Fecha de inicio"),
        new("field.end_date", Fields, null, "Data de fim", "End date", "Fecha de fin"),
        new("field.initial_mileage", Fields, null, "Quilometragem inicial", "Initial mileage", "Kilometraje inicial"),
        new("field.period", Fields, null, "Período", "Period", "Periodo"),
        new("field.key", Fields, null, "Chave", "Key", "Clave"),
        new("field.category", Fields, null, "Categoria", "Category", "Categoría"),
        new("field.description", Fields, null, "Descrição", "Description", "Descripción"),
        new("field.translation_notes", Fields, null, "Descrição (onde aparece / notas)", "Description (where it appears / notes)", "Descripción (dónde aparece / notas)"),

        // ---------- Validação ----------
        new("validation.email_required", Validation, null, "O email é obrigatório.", "Email is required.", "El correo es obligatorio."),
        new("validation.password_required", Validation, null, "A palavra-passe é obrigatória.", "Password is required.", "La contraseña es obligatoria."),
        new("validation.full_name_required", Validation, null, "O nome completo é obrigatório.", "Full name is required.", "El nombre completo es obligatorio."),
        new("validation.password_mismatch", Validation, null, "As palavras-passe não coincidem.", "The passwords don't match.", "Las contraseñas no coinciden."),
        new("validation.current_password_required", Validation, null, "Indica a palavra-passe atual.", "Enter your current password.", "Indica la contraseña actual."),
        new("validation.new_password_required", Validation, null, "Indica a nova palavra-passe.", "Enter a new password.", "Indica la nueva contraseña."),
        new("validation.confirm_new_password", Validation, null, "Confirma a nova palavra-passe.", "Confirm the new password.", "Confirma la nueva contraseña."),
        new("validation.phone_required", Validation, null, "O telefone é obrigatório.", "The phone number is required.", "El teléfono es obligatorio."),
        new("validation.phone_format", Validation, null, "O telefone deve conter 9 dígitos numéricos.", "The phone number must contain 9 digits.", "El teléfono debe contener 9 dígitos."),
        new("validation.bio_length", Validation, null, "A bio não pode ter mais de 500 caracteres.", "The bio can't exceed 500 characters.", "La biografía no puede superar los 500 caracteres."),
        new("validation.license_required", Validation, null, "A carta de condução é obrigatória.", "The driving licence number is required.", "El permiso de conducir es obligatorio."),
        new("validation.brand_required", Validation, null, "A marca é obrigatória.", "The brand is required.", "La marca es obligatoria."),
        new("validation.model_required", Validation, null, "O modelo é obrigatório.", "The model is required.", "El modelo es obligatorio."),
        new("validation.registration_required", Validation, null, "A matrícula é obrigatória.", "The registration is required.", "La matrícula es obligatoria."),
        new("validation.year_invalid", Validation, null, "Indica um ano de fabrico válido.", "Enter a valid year of manufacture.", "Indica un año de fabricación válido."),
        new("validation.year_future", Validation, null, "O ano de fabrico não pode ser posterior ao ano atual.", "The year of manufacture can't be later than the current year.", "El año de fabricación no puede ser posterior al año actual."),
        new("validation.fuel_required", Validation, null, "Seleciona um tipo de combustível.", "Select a fuel type.", "Selecciona un tipo de combustible."),
        new("validation.mileage_range", Validation, null, "A quilometragem tem de estar entre 0 e 2 000 000 km.", "The mileage must be between 0 and 2,000,000 km.", "El kilometraje debe estar entre 0 y 2 000 000 km."),
        new("validation.customer_required", Validation, null, "Seleciona um cliente.", "Select a customer.", "Selecciona un cliente."),
        new("validation.customer_invalid", Validation, null, "Seleciona um cliente válido.", "Select a valid customer.", "Selecciona un cliente válido."),
        new("validation.vehicle_required", Validation, null, "Seleciona um veículo.", "Select a vehicle.", "Selecciona un vehículo."),
        new("validation.vehicle_invalid", Validation, null, "Seleciona um veículo válido.", "Select a valid vehicle.", "Selecciona un vehículo válido."),
        new("validation.start_required", Validation, null, "A data de início é obrigatória.", "The start date is required.", "La fecha de inicio es obligatoria."),
        new("validation.end_required", Validation, null, "A data de fim é obrigatória.", "The end date is required.", "La fecha de fin es obligatoria."),
        new("validation.initial_mileage_required", Validation, null, "A quilometragem inicial é obrigatória.", "The initial mileage is required.", "El kilometraje inicial es obligatorio."),
        new("validation.initial_mileage_negative", Validation, null, "A quilometragem inicial não pode ser negativa.", "The initial mileage can't be negative.", "El kilometraje inicial no puede ser negativo."),
        new("validation.start_past", Validation, null, "A data de início não pode ser anterior à data atual.", "The start date can't be earlier than today.", "La fecha de inicio no puede ser anterior a hoy."),
        new("validation.end_after_start", Validation, null, "A data de fim tem de ser posterior à data de início.", "The end date must be after the start date.", "La fecha de fin debe ser posterior a la de inicio."),
        new("validation.customer_missing", Validation, null, "O cliente selecionado não existe.", "The selected customer doesn't exist.", "El cliente seleccionado no existe."),
        new("validation.vehicle_missing", Validation, null, "O veículo selecionado não existe.", "The selected vehicle doesn't exist.", "El vehículo seleccionado no existe."),
        new("validation.rental_overlap", Validation, null, "Este veículo já tem um contrato que se sobrepõe ao período indicado.", "This vehicle already has a contract that overlaps the given period.", "Este vehículo ya tiene un contrato que se solapa con el periodo indicado."),
        new("validation.vehicle_duplicate", Validation, null, "Já existe um veículo com esta matrícula.", "A vehicle with this registration already exists.", "Ya existe un vehículo con esta matrícula."),
        new("validation.customer_duplicate", Validation, null, "Já existe um cliente com este email.", "A customer with this email already exists.", "Ya existe un cliente con este correo."),
        new("validation.key_required", Validation, null, "A chave é obrigatória.", "The key is required.", "La clave es obligatoria."),
        new("validation.key_format", Validation, null, "Usa letras minúsculas, números e pontos (ex.: dash.hero.title).", "Use lowercase letters, numbers and dots (e.g. dash.hero.title).", "Usa letras minúsculas, números y puntos (ej.: dash.hero.title)."),
        new("validation.category_required", Validation, null, "A categoria é obrigatória.", "The category is required.", "La categoría es obligatoria."),
        new("validation.translation_duplicate", Validation, null, "Já existe uma tradução com esta chave.", "A translation with this key already exists.", "Ya existe una traducción con esta clave."),
        new("validation.translation_pt_required", Validation, null, "O texto em português é obrigatório (é o idioma de recurso).", "The Portuguese text is required (it is the fallback language).", "El texto en portugués es obligatorio (es el idioma de respaldo)."),

        // ---------- Combustível ----------
        new("fuel.Unspecified", Fuel, null, "Por selecionar", "Not specified", "Sin especificar"),
        new("fuel.Petrol", Fuel, null, "Gasolina", "Petrol", "Gasolina"),
        new("fuel.Diesel", Fuel, null, "Gasóleo", "Diesel", "Diésel"),
        new("fuel.Hybrid", Fuel, null, "Híbrido", "Hybrid", "Híbrido"),
        new("fuel.Electric", Fuel, null, "Elétrico", "Electric", "Eléctrico"),
        new("fuel.Lpg", Fuel, null, "GPL", "LPG", "GLP"),
        new("fuel.Other", Fuel, null, "Outro", "Other", "Otro")
    ];

    /// <summary>
    /// Chaves do antigo site público (já removido). O arranque apaga-as da base de dados se ainda lá estiverem,
    /// para não ficarem textos órfãos na página de Traduções.
    /// </summary>
    public static readonly IReadOnlyList<string> RetiredPrefixes =
    [
        "home.", "fleet.", "vehicle.", "booking.", "bookings.", "footer.", "carousel.", "layout."
    ];

    public static readonly IReadOnlyList<string> RetiredKeys =
    [
        "common.available_today", "common.see_all_vehicles", "common.pickup", "common.return", "common.brand", "common.fuel",
        "common.all_brands", "common.all_masc", "common.all_fem",
        "nav.home", "nav.fleet", "nav.how", "nav.contacts", "nav.my_bookings", "nav.admin", "nav.register", "nav.account_menu", "nav.open_menu",
        "account.create", "account.login.no_account", "account.register.eyebrow", "account.register.lead", "account.register.has_account",
        "account.error.duplicate",
        "field.confirm_password", "validation.confirm_password"
    ];

    public static bool IsRetired(string key)
        => RetiredKeys.Contains(key) || RetiredPrefixes.Any(prefix => key.StartsWith(prefix, StringComparison.Ordinal));

    private static readonly Dictionary<string, CatalogEntry> ByKey = Entries.ToDictionary(entry => entry.Key, StringComparer.Ordinal);

    public static CatalogEntry? Find(string key) => ByKey.GetValueOrDefault(key);

    public static string? DefaultFor(string key, string language) => Find(key)?.For(language);
}
