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
/// Textos do site com os valores iniciais em PT/EN/ES. Ao arrancar, as chaves em falta são inseridas na
/// base de dados (sem alterar o que já foi editado na administração); depois, estes valores só servem de recurso.
/// Para adicionar um texto novo: criar a entrada aqui e usar <c>@T["chave"]</c> na vista.
/// </summary>
public static class TranslationCatalog
{
    private const string Layout = "Layout";
    private const string Home = "Página inicial";
    private const string Fleet = "Frota";
    private const string Vehicle = "Viatura";
    private const string Bookings = "Reservas";
    private const string Account = "Conta";
    private const string Profile = "Perfil";
    private const string Fields = "Campos de formulário";
    private const string Validation = "Validação";
    private const string Fuel = "Combustível";
    private const string Common = "Geral";

    public static readonly IReadOnlyList<CatalogEntry> Entries =
    [
        // ---------- Geral ----------
        new("common.cancel", Common, null, "Cancelar", "Cancel", "Cancelar"),
        new("common.back_home", Common, null, "Voltar ao início", "Back to home", "Volver al inicio"),
        new("common.available_today", Common, "Usado em títulos, filtros e no rodapé", "Disponíveis hoje", "Available today", "Disponibles hoy"),
        new("common.see_all_vehicles", Common, null, "Ver todas as viaturas", "See all vehicles", "Ver todos los vehículos"),
        new("common.pickup", Common, "Data de levantamento", "Levantamento", "Pick-up", "Recogida"),
        new("common.return", Common, "Data de devolução", "Devolução", "Return", "Devolución"),
        new("common.brand", Common, null, "Marca", "Brand", "Marca"),
        new("common.fuel", Common, null, "Combustível", "Fuel", "Combustible"),
        new("common.all_brands", Common, null, "Todas as marcas", "All brands", "Todas las marcas"),
        new("common.all_masc", Common, "Opção \"todos\" (masculino)", "Todos", "All", "Todos"),
        new("common.all_fem", Common, "Opção \"todas\" (feminino)", "Todas", "All", "Todas"),
        new("error.title", Common, "Página de erro", "Ocorreu um erro", "Something went wrong", "Se produjo un error"),
        new("error.text", Common, "Página de erro", "Não foi possível processar o teu pedido. Tenta novamente dentro de momentos.", "We couldn't process your request. Please try again in a moment.", "No hemos podido procesar tu solicitud. Inténtalo de nuevo en unos momentos."),

        // ---------- Layout (cabeçalho e rodapé) ----------
        new("layout.meta.description", Layout, "Descrição para motores de busca", "Aluguer de viaturas GrupoJap: pesquisa a frota, confirma a disponibilidade e reserva online.", "GrupoJap car rental: browse the fleet, check availability and book online.", "Alquiler de vehículos GrupoJap: consulta la flota, comprueba la disponibilidad y reserva online."),
        new("layout.skip", Layout, "Link de acessibilidade", "Saltar para o conteúdo", "Skip to content", "Saltar al contenido"),
        new("layout.logo_label", Layout, "Texto do logótipo para leitores de ecrã", "GrupoJap Rent-a-Car, página inicial", "GrupoJap Rent-a-Car, home page", "GrupoJap Rent-a-Car, página de inicio"),
        new("nav.label", Layout, null, "Navegação principal", "Main navigation", "Navegación principal"),
        new("nav.home", Layout, "Menu principal", "Início", "Home", "Inicio"),
        new("nav.fleet", Layout, "Menu principal", "Viaturas", "Vehicles", "Vehículos"),
        new("nav.how", Layout, "Menu principal", "Como funciona", "How it works", "Cómo funciona"),
        new("nav.contacts", Layout, "Menu principal", "Contactos", "Contact", "Contacto"),
        new("nav.my_bookings", Layout, null, "As minhas reservas", "My bookings", "Mis reservas"),
        new("nav.my_profile", Layout, null, "O meu perfil", "My profile", "Mi perfil"),
        new("nav.admin", Layout, null, "Painel de administração", "Admin panel", "Panel de administración"),
        new("nav.logout", Layout, null, "Terminar sessão", "Sign out", "Cerrar sesión"),
        new("nav.login", Layout, null, "Entrar", "Sign in", "Iniciar sesión"),
        new("nav.register", Layout, null, "Registar", "Sign up", "Registrarse"),
        new("nav.account_menu", Layout, null, "Menu da conta", "Account menu", "Menú de la cuenta"),
        new("nav.open_menu", Layout, null, "Abrir menu", "Open menu", "Abrir menú"),
        new("nav.language", Layout, "Seletor de idioma", "Idioma", "Language", "Idioma"),
        new("footer.about", Layout, "Texto do rodapé", "Aluguer de viaturas multimarca com reserva online e disponibilidade confirmada em tempo real.", "Multi-brand car rental with online booking and availability confirmed in real time.", "Alquiler de vehículos multimarca con reserva online y disponibilidad confirmada en tiempo real."),
        new("footer.explore", Layout, null, "Explorar", "Explore", "Explorar"),
        new("footer.account", Layout, null, "Conta", "Account", "Cuenta"),
        new("footer.contacts", Layout, null, "Contactos", "Contact", "Contacto"),
        new("footer.phone", Layout, null, "Telefone", "Phone", "Teléfono"),
        new("footer.email", Layout, null, "Email", "Email", "Correo"),
        new("footer.address", Layout, null, "Morada", "Address", "Dirección"),
        new("footer.phone_value", Layout, "Número de telefone mostrado no rodapé", "a definir", "to be defined", "por definir"),
        new("footer.email_value", Layout, "Email mostrado no rodapé", "a definir", "to be defined", "por definir"),
        new("footer.address_value", Layout, "Morada mostrada no rodapé", "a definir", "to be defined", "por definir"),
        new("footer.copyright", Layout, "Depois de \"© ano GrupoJap ·\"", "Projeto de avaliação técnica", "Technical assessment project", "Proyecto de evaluación técnica"),
        new("footer.made_with", Layout, null, "Feito com ASP.NET Core MVC", "Built with ASP.NET Core MVC", "Hecho con ASP.NET Core MVC"),
        new("carousel.prev", Layout, "Botão do carrossel", "Página anterior", "Previous page", "Página anterior"),
        new("carousel.next", Layout, "Botão do carrossel", "Página seguinte", "Next page", "Página siguiente"),
        new("carousel.goto", Layout, "{0} = página, {1} = total", "Ir para a página {0} de {1}", "Go to page {0} of {1}", "Ir a la página {0} de {1}"),

        // ---------- Página inicial ----------
        new("home.title", Home, "Título do separador", "Aluguer de viaturas", "Car rental", "Alquiler de vehículos"),
        new("home.hero.kicker", Home, null, "Rent-a-car multimarca", "Multi-brand rent-a-car", "Rent-a-car multimarca"),
        new("home.hero.title", Home, "Título principal, antes do destaque a verde", "Encontra a viatura certa e reserva", "Find the right car and book it", "Encuentra el vehículo ideal y resérvalo"),
        new("home.hero.highlight", Home, "Parte do título a verde", "em minutos", "in minutes", "en minutos"),
        new("home.hero.lead", Home, null, "Pesquisa a frota, vê a disponibilidade real para as tuas datas e confirma a reserva online, sem filas nem papelada.", "Browse the fleet, see real availability for your dates and confirm your booking online, with no queues or paperwork.", "Consulta la flota, comprueba la disponibilidad real para tus fechas y confirma la reserva online, sin colas ni papeleo."),
        new("home.stats.fleet", Home, null, "Viaturas na frota", "Vehicles in the fleet", "Vehículos en la flota"),
        new("home.stats.brands", Home, null, "Marcas", "Brands", "Marcas"),
        new("home.search.title", Home, null, "Pesquisar disponibilidade", "Check availability", "Buscar disponibilidad"),
        new("home.search.submit", Home, null, "Ver viaturas disponíveis", "See available vehicles", "Ver vehículos disponibles"),
        new("home.popular", Home, null, "Pesquisas populares:", "Popular searches:", "Búsquedas populares:"),
        new("home.featured.kicker", Home, null, "Destaques", "Featured", "Destacados"),
        new("home.featured.empty_title", Home, null, "Sem viaturas disponíveis hoje", "No vehicles available today", "No hay vehículos disponibles hoy"),
        new("home.featured.empty_text", Home, null, "Experimenta pesquisar outras datas na frota completa.", "Try searching other dates in the full fleet.", "Prueba a buscar otras fechas en la flota completa."),
        new("home.featured.empty_cta", Home, null, "Ver frota", "View fleet", "Ver flota"),
        new("home.brands.title", Home, null, "Escolhe pela marca", "Choose by brand", "Elige por marca"),
        new("home.brands.count", Home, "{0} = número de viaturas", "{0} viatura(s)", "{0} vehicle(s)", "{0} vehículo(s)"),
        new("home.how.title", Home, null, "Três passos até à estrada", "Three steps to the road", "Tres pasos hasta la carretera"),
        new("home.how.step1_title", Home, null, "Pesquisa", "Search", "Busca"),
        new("home.how.step1_text", Home, null, "Indica as datas de levantamento e devolução e vê de imediato que viaturas estão livres.", "Enter your pick-up and return dates and instantly see which vehicles are free.", "Indica las fechas de recogida y devolución y ve al instante qué vehículos están libres."),
        new("home.how.step2_title", Home, null, "Reserva", "Book", "Reserva"),
        new("home.how.step2_text", Home, null, "Cria conta, escolhe a viatura e confirma. Garantimos que o período não colide com outra reserva.", "Create an account, choose your vehicle and confirm. We make sure the dates don't clash with another booking.", "Crea una cuenta, elige el vehículo y confirma. Garantizamos que el periodo no coincide con otra reserva."),
        new("home.how.step3_title", Home, null, "Conduz", "Drive", "Conduce"),
        new("home.how.step3_text", Home, null, "Levanta a viatura na data marcada. Acompanha e gere as tuas reservas na tua área de cliente.", "Pick up the vehicle on the scheduled date. Track and manage your bookings in your customer area.", "Recoge el vehículo en la fecha prevista. Sigue y gestiona tus reservas en tu área de cliente."),
        new("home.why.kicker", Home, null, "Porquê nós", "Why us", "Por qué nosotros"),
        new("home.why.title", Home, null, "Aluguer simples, transparente e sem surpresas.", "Simple, transparent rental with no surprises.", "Alquiler sencillo, transparente y sin sorpresas."),
        new("home.why.lead", Home, null, "Uma frota multimarca gerida ao detalhe, com disponibilidade calculada a partir dos contratos reais.", "A multi-brand fleet managed in detail, with availability calculated from real contracts.", "Una flota multimarca gestionada al detalle, con disponibilidad calculada a partir de los contratos reales."),
        new("home.why.item1_title", Home, null, "Frota multimarca", "Multi-brand fleet", "Flota multimarca"),
        new("home.why.item1_text", Home, null, "Citadinos, familiares e elétricos para cada necessidade.", "City cars, family cars and EVs for every need.", "Urbanos, familiares y eléctricos para cada necesidad."),
        new("home.why.item2_title", Home, null, "Disponibilidade real", "Real availability", "Disponibilidad real"),
        new("home.why.item2_text", Home, null, "O estado de cada viatura é atualizado automaticamente a partir das reservas.", "Each vehicle's status is updated automatically from bookings.", "El estado de cada vehículo se actualiza automáticamente a partir de las reservas."),
        new("home.why.item3_title", Home, null, "Reserva online", "Online booking", "Reserva online"),
        new("home.why.item3_text", Home, null, "Reserva em minutos e cancela gratuitamente antes do início do aluguer.", "Book in minutes and cancel for free before the rental starts.", "Reserva en minutos y cancela gratis antes del inicio del alquiler."),
        new("home.why.item4_title", Home, null, "Área de cliente", "Customer area", "Área de cliente"),
        new("home.why.item4_text", Home, null, "Todas as tuas reservas, passadas e futuras, num só lugar.", "All your bookings, past and future, in one place.", "Todas tus reservas, pasadas y futuras, en un solo lugar."),
        new("home.cta.title", Home, null, "Pronto para a próxima viagem?", "Ready for your next trip?", "¿Listo para tu próximo viaje?"),
        new("home.cta.text", Home, null, "Cria a tua conta gratuita e faz a primeira reserva hoje.", "Create your free account and make your first booking today.", "Crea tu cuenta gratuita y haz tu primera reserva hoy."),
        new("home.cta.explore", Home, null, "Explorar viaturas", "Explore vehicles", "Explorar vehículos"),

        // ---------- Frota ----------
        new("fleet.kicker", Fleet, null, "Frota", "Fleet", "Flota"),
        new("fleet.heading", Fleet, null, "Viaturas para alugar", "Vehicles for rent", "Vehículos en alquiler"),
        new("fleet.count", Fleet, "{0} = número de resultados", "{0} viatura(s) encontrada(s)", "{0} vehicle(s) found", "{0} vehículo(s) encontrado(s)"),
        new("fleet.period", Fleet, "{0} = data de início, {1} = data de fim", "disponibilidade de {0} a {1}", "availability from {0} to {1}", "disponibilidad del {0} al {1}"),
        new("fleet.today", Fleet, null, "disponibilidade para hoje", "availability for today", "disponibilidad para hoy"),
        new("fleet.filters", Fleet, null, "Filtros", "Filters", "Filtros"),
        new("fleet.filter", Fleet, null, "Filtrar", "Filter", "Filtrar"),
        new("fleet.clear", Fleet, null, "Limpar", "Clear", "Limpiar"),
        new("fleet.search", Fleet, null, "Pesquisa", "Search", "Búsqueda"),
        new("fleet.search_placeholder", Fleet, null, "Marca ou modelo", "Brand or model", "Marca o modelo"),
        new("fleet.period_legend", Fleet, null, "Período", "Period", "Periodo"),
        new("fleet.only_available", Fleet, null, "Só disponíveis", "Available only", "Solo disponibles"),
        new("fleet.sort", Fleet, null, "Ordenar por", "Sort by", "Ordenar por"),
        new("fleet.sort.Brand", Fleet, null, "Marca (A-Z)", "Brand (A-Z)", "Marca (A-Z)"),
        new("fleet.sort.Newest", Fleet, null, "Mais recentes", "Newest", "Más recientes"),
        new("fleet.sort.LowestMileage", Fleet, null, "Menos quilómetros", "Lowest mileage", "Menos kilómetros"),
        new("fleet.apply", Fleet, null, "Aplicar filtros", "Apply filters", "Aplicar filtros"),
        new("fleet.results", Fleet, null, "Resultados", "Results", "Resultados"),
        new("fleet.empty_title", Fleet, null, "Nenhuma viatura corresponde à pesquisa", "No vehicles match your search", "Ningún vehículo coincide con la búsqueda"),
        new("fleet.empty_text", Fleet, null, "Experimenta outras datas ou remove alguns filtros.", "Try other dates or remove some filters.", "Prueba otras fechas o quita algunos filtros."),
        new("fleet.pagination", Fleet, null, "Paginação", "Pagination", "Paginación"),
        new("fleet.prev", Fleet, null, "← Anterior", "← Previous", "← Anterior"),
        new("fleet.next", Fleet, null, "Seguinte →", "Next →", "Siguiente →"),
        new("fleet.error.both_dates", Fleet, null, "Indica as duas datas para pesquisar por período.", "Enter both dates to search by period.", "Indica ambas fechas para buscar por periodo."),
        new("fleet.error.start_past", Fleet, null, "A data de início não pode ser anterior a hoje.", "The start date can't be earlier than today.", "La fecha de inicio no puede ser anterior a hoy."),

        // ---------- Viatura (cartões e detalhe) ----------
        new("vehicle.available", Vehicle, null, "Disponível", "Available", "Disponible"),
        new("vehicle.rented", Vehicle, null, "Alugado", "Rented", "Alquilado"),
        new("vehicle.details", Vehicle, null, "Ver detalhes", "View details", "Ver detalles"),
        new("vehicle.available_today", Vehicle, null, "Disponível hoje", "Available today", "Disponible hoy"),
        new("vehicle.rented_today", Vehicle, null, "Alugado hoje", "Rented today", "Alquilado hoy"),
        new("vehicle.year", Vehicle, null, "Ano", "Year", "Año"),
        new("vehicle.mileage", Vehicle, null, "Quilometragem", "Mileage", "Kilometraje"),
        new("vehicle.booked_title", Vehicle, null, "Datas já reservadas", "Already booked dates", "Fechas ya reservadas"),
        new("vehicle.booked_none", Vehicle, null, "Sem reservas futuras. Esta viatura está livre em qualquer data.", "No upcoming bookings. This vehicle is free on any date.", "Sin reservas futuras. Este vehículo está libre en cualquier fecha."),
        new("vehicle.booked_hint", Vehicle, null, "Escolhe um período que não coincida com estas datas (inclusive).", "Choose a period that doesn't overlap these dates (inclusive).", "Elige un periodo que no coincida con estas fechas (inclusive)."),
        new("vehicle.related_kicker", Vehicle, null, "Sugestões", "Suggestions", "Sugerencias"),
        new("vehicle.related_title", Vehicle, null, "Também te pode interessar", "You may also like", "También te puede interesar"),

        // ---------- Reservas ----------
        new("booking.title", Bookings, null, "Reservar esta viatura", "Book this vehicle", "Reservar este vehículo"),
        new("booking.login_hint", Bookings, null, "Inicia sessão ou cria conta para concluíres a reserva.", "Sign in or create an account to complete your booking.", "Inicia sesión o crea una cuenta para completar la reserva."),
        new("booking.login_cta", Bookings, null, "Entrar para reservar", "Sign in to book", "Inicia sesión para reservar"),
        new("booking.first_note", Bookings, null, "Primeira reserva: precisamos destes dados para o contrato.", "First booking: we need these details for the contract.", "Primera reserva: necesitamos estos datos para el contrato."),
        new("booking.phone_placeholder", Bookings, null, "9 dígitos", "9 digits", "9 dígitos"),
        new("booking.submit", Bookings, null, "Confirmar reserva", "Confirm booking", "Confirmar reserva"),
        new("booking.cancel_policy", Bookings, null, "Cancelamento gratuito até ao dia anterior ao levantamento.", "Free cancellation until the day before pick-up.", "Cancelación gratuita hasta el día anterior a la recogida."),
        new("booking.days_label", Bookings, "{0} = número de dias", "{0} dia(s) de aluguer", "{0} rental day(s)", "{0} día(s) de alquiler"),
        new("booking.success", Bookings, null, "Reserva confirmada! Encontras todos os detalhes abaixo.", "Booking confirmed! You'll find all the details below.", "¡Reserva confirmada! Encontrarás todos los detalles abajo."),
        new("booking.cancelled", Bookings, null, "Reserva cancelada.", "Booking cancelled.", "Reserva cancelada."),
        new("booking.cancel_not_allowed", Bookings, null, "Só é possível cancelar reservas que ainda não começaram.", "Only bookings that haven't started yet can be cancelled.", "Solo se pueden cancelar reservas que aún no han empezado."),
        new("booking.error.dates_required", Bookings, null, "Indica as datas de início e de fim.", "Enter the start and end dates.", "Indica las fechas de inicio y fin."),
        new("booking.error.start_past", Bookings, null, "A data de início não pode ser anterior à data atual.", "The start date can't be earlier than today.", "La fecha de inicio no puede ser anterior a hoy."),
        new("booking.error.end_before_start", Bookings, null, "A data de fim tem de ser posterior à data de início.", "The end date must be after the start date.", "La fecha de fin debe ser posterior a la de inicio."),
        new("booking.error.vehicle_missing", Bookings, null, "O veículo selecionado não existe.", "The selected vehicle doesn't exist.", "El vehículo seleccionado no existe."),
        new("booking.error.phone_required", Bookings, null, "O telefone é obrigatório.", "The phone number is required.", "El teléfono es obligatorio."),
        new("booking.error.license_required", Bookings, null, "A carta de condução é obrigatória.", "The driving licence number is required.", "El permiso de conducir es obligatorio."),
        new("booking.error.overlap", Bookings, null, "Este veículo já está reservado em parte desse período. Escolhe outras datas.", "This vehicle is already booked for part of that period. Choose other dates.", "Este vehículo ya está reservado en parte de ese periodo. Elige otras fechas."),
        new("booking.error.generic", Bookings, null, "Não foi possível concluir a reserva. Tenta novamente.", "We couldn't complete the booking. Please try again.", "No se ha podido completar la reserva. Inténtalo de nuevo."),
        new("bookings.kicker", Bookings, null, "Área de cliente", "Customer area", "Área de cliente"),
        new("bookings.hello", Bookings, "{0} = nome do cliente", "Olá, {0}", "Hi, {0}", "Hola, {0}"),
        new("bookings.lead", Bookings, null, "Aqui encontras todas as tuas reservas.", "Here you'll find all your bookings.", "Aquí encontrarás todas tus reservas."),
        new("bookings.empty_title", Bookings, null, "Ainda não tens reservas", "You don't have any bookings yet", "Aún no tienes reservas"),
        new("bookings.empty_text", Bookings, null, "Pesquisa a frota e faz a tua primeira reserva em poucos minutos.", "Browse the fleet and make your first booking in just a few minutes.", "Consulta la flota y haz tu primera reserva en pocos minutos."),
        new("bookings.empty_cta", Bookings, null, "Ver viaturas", "View vehicles", "Ver vehículos"),
        new("bookings.status.active", Bookings, null, "Em curso", "In progress", "En curso"),
        new("bookings.status.upcoming", Bookings, null, "Agendada", "Scheduled", "Programada"),
        new("bookings.status.past", Bookings, null, "Concluída", "Completed", "Finalizada"),
        new("bookings.days", Bookings, "{0} = número de dias", "{0} dia(s)", "{0} day(s)", "{0} día(s)"),
        new("bookings.cancel", Bookings, null, "Cancelar reserva", "Cancel booking", "Cancelar reserva"),
        new("bookings.cancel_confirm", Bookings, "Pergunta de confirmação", "Cancelar esta reserva?", "Cancel this booking?", "¿Cancelar esta reserva?"),

        // ---------- Conta ----------
        new("account.create", Account, null, "Criar conta", "Create account", "Crear cuenta"),
        new("account.login.eyebrow", Account, null, "ACESSO", "ACCESS", "ACCESO"),
        new("account.login.lead", Account, null, "Inicia sessão para continuares.", "Sign in to continue.", "Inicia sesión para continuar."),
        new("account.login.no_account", Account, null, "Ainda não tens conta?", "Don't have an account yet?", "¿Aún no tienes cuenta?"),
        new("account.register.eyebrow", Account, null, "NOVA CONTA", "NEW ACCOUNT", "NUEVA CUENTA"),
        new("account.register.lead", Account, null, "A palavra-passe precisa de 8 caracteres, maiúscula, minúscula, número e símbolo.", "Your password needs 8 characters, including an uppercase letter, a lowercase letter, a number and a symbol.", "La contraseña necesita 8 caracteres, con mayúscula, minúscula, número y símbolo."),
        new("account.register.has_account", Account, null, "Já tens conta?", "Already have an account?", "¿Ya tienes cuenta?"),
        new("account.denied.title", Account, null, "Acesso negado", "Access denied", "Acceso denegado"),
        new("account.denied.heading", Account, null, "Sem permissão", "No permission", "Sin permiso"),
        new("account.denied.lead", Account, null, "Esta área é reservada a administradores.", "This area is restricted to administrators.", "Esta área está reservada a administradores."),
        new("account.error.invalid", Account, null, "Email ou palavra-passe incorretos.", "Incorrect email or password.", "Correo o contraseña incorrectos."),
        new("account.error.locked", Account, null, "Conta temporariamente bloqueada por demasiadas tentativas. Tenta novamente dentro de alguns minutos.", "Account temporarily locked after too many attempts. Try again in a few minutes.", "Cuenta bloqueada temporalmente por demasiados intentos. Inténtalo de nuevo en unos minutos."),
        new("account.error.duplicate", Account, null, "Já existe uma conta com este email.", "An account with this email already exists.", "Ya existe una cuenta con este correo."),
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
        new("field.confirm_password", Fields, null, "Confirmar palavra-passe", "Confirm password", "Confirmar contraseña"),
        new("field.phone", Fields, null, "Telefone", "Phone", "Teléfono"),
        new("field.job_title", Fields, null, "Cargo", "Job title", "Cargo"),
        new("field.favorite_brand", Fields, null, "Marca favorita", "Favourite brand", "Marca favorita"),
        new("field.bio", Fields, null, "Sobre mim", "About me", "Sobre mí"),
        new("field.photo", Fields, null, "Nova foto", "New photo", "Nueva foto"),
        new("field.remove_photo", Fields, null, "Remover foto atual", "Remove current photo", "Eliminar foto actual"),
        new("field.current_password", Fields, null, "Palavra-passe atual", "Current password", "Contraseña actual"),
        new("field.new_password", Fields, null, "Nova palavra-passe", "New password", "Nueva contraseña"),
        new("field.confirm_new_password", Fields, null, "Confirmar nova palavra-passe", "Confirm new password", "Confirmar nueva contraseña"),
        new("field.start_date", Fields, null, "Data de início", "Start date", "Fecha de inicio"),
        new("field.end_date", Fields, null, "Data de fim", "End date", "Fecha de fin"),
        new("field.driving_license", Fields, null, "Carta de condução", "Driving licence", "Permiso de conducir"),

        // ---------- Validação ----------
        new("validation.email_required", Validation, null, "O email é obrigatório.", "Email is required.", "El correo es obligatorio."),
        new("validation.password_required", Validation, null, "A palavra-passe é obrigatória.", "Password is required.", "La contraseña es obligatoria."),
        new("validation.full_name_required", Validation, null, "O nome completo é obrigatório.", "Full name is required.", "El nombre completo es obligatorio."),
        new("validation.confirm_password", Validation, null, "Confirma a palavra-passe.", "Confirm your password.", "Confirma la contraseña."),
        new("validation.password_mismatch", Validation, null, "As palavras-passe não coincidem.", "The passwords don't match.", "Las contraseñas no coinciden."),
        new("validation.current_password_required", Validation, null, "Indica a palavra-passe atual.", "Enter your current password.", "Indica la contraseña actual."),
        new("validation.new_password_required", Validation, null, "Indica a nova palavra-passe.", "Enter a new password.", "Indica la nueva contraseña."),
        new("validation.confirm_new_password", Validation, null, "Confirma a nova palavra-passe.", "Confirm the new password.", "Confirma la nueva contraseña."),
        new("validation.phone_format", Validation, null, "O telefone deve conter 9 dígitos numéricos.", "The phone number must contain 9 digits.", "El teléfono debe contener 9 dígitos."),
        new("validation.bio_length", Validation, null, "A bio não pode ter mais de 500 caracteres.", "The bio can't exceed 500 characters.", "La biografía no puede superar los 500 caracteres."),
        new("validation.start_required", Validation, null, "A data de início é obrigatória.", "The start date is required.", "La fecha de inicio es obligatoria."),
        new("validation.end_required", Validation, null, "A data de fim é obrigatória.", "The end date is required.", "La fecha de fin es obligatoria."),

        // ---------- Combustível ----------
        new("fuel.Unspecified", Fuel, null, "Por selecionar", "Not specified", "Sin especificar"),
        new("fuel.Petrol", Fuel, null, "Gasolina", "Petrol", "Gasolina"),
        new("fuel.Diesel", Fuel, null, "Gasóleo", "Diesel", "Diésel"),
        new("fuel.Hybrid", Fuel, null, "Híbrido", "Hybrid", "Híbrido"),
        new("fuel.Electric", Fuel, null, "Elétrico", "Electric", "Eléctrico"),
        new("fuel.Lpg", Fuel, null, "GPL", "LPG", "GLP"),
        new("fuel.Other", Fuel, null, "Outro", "Other", "Otro")
    ];

    private static readonly Dictionary<string, CatalogEntry> ByKey = Entries.ToDictionary(entry => entry.Key, StringComparer.Ordinal);

    public static CatalogEntry? Find(string key) => ByKey.GetValueOrDefault(key);

    public static string? DefaultFor(string key, string language) => Find(key)?.For(language);
}
