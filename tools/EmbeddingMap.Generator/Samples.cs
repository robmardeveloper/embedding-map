namespace EmbeddingMap.Generator;

/// <summary>A sample set before it has embeddings: each group and its texts.</summary>
internal sealed record SampleSet(string Id, string Name, string Description, IReadOnlyList<(string Group, string[] Texts)> Groups);

/// <summary>The texts the page shows. All of them are invented for this demo.</summary>
internal static class Samples
{
    public static readonly SampleSet[] All =
    [
        new("topics", "Mixed topics", "Short phrases about four topics. Texts on the same topic end up close together.",
        [
            ("Animals",
            [
                "a dog chasing a ball in the park", "the cat sleeps on the sofa all afternoon", "a horse running across a field",
                "an eagle flying high over the mountains", "a goldfish swimming in a small bowl", "cows grazing in a green meadow",
                "a puppy learning to sit", "the lion is the king of the savannah", "a rabbit hiding in the grass",
                "dolphins jumping out of the sea", "a parrot that can say hello", "bees collecting pollen from flowers",
                "a turtle walking very slowly", "wolves howling at night", "feeding the ducks at the pond", "a squirrel storing nuts for winter",
            ]),
            ("Food",
            [
                "a slice of pizza with extra cheese", "fresh bread straight from the oven", "a bowl of tomato soup",
                "grilled chicken with roasted vegetables", "chocolate cake for dessert", "a cup of strong black coffee",
                "pasta with garlic and olive oil", "a fruit salad with strawberries", "scrambled eggs on toast",
                "a spicy curry with rice", "homemade lemonade on a hot day", "vanilla ice cream in a cone",
                "a cheese and ham sandwich", "pancakes with maple syrup", "a green salad with avocado", "sushi rolls with soy sauce",
            ]),
            ("Programming",
            [
                "fixing a bug in the login page", "writing unit tests for the new feature", "the build failed on the server",
                "merging a pull request", "refactoring a long function", "a null reference exception in production",
                "adding an index to speed up the query", "reviewing a colleague's code", "deploying the app to the cloud",
                "the API returns a 404 error", "upgrading a library to the latest version", "debugging with breakpoints",
                "writing a script to automate the backup", "the tests pass on my machine", "designing a database schema", "a memory leak in the service",
            ]),
            ("Travel",
            [
                "a flight to Rome next weekend", "packing a suitcase for the holidays", "a hotel room with a sea view",
                "the train leaves at eight in the morning", "visiting a museum in Paris", "a road trip along the coast",
                "lost luggage at the airport", "renting a car for a week", "a guided tour of the old town",
                "booking a cheap hostel", "the passport expires next month", "camping near a lake", "a long queue at security",
                "exploring the streets of Lisbon", "a ferry to the island", "jet lag after a long flight",
            ]),
        ]),

        new("languages", "Spanish and English", "The same words in both languages. Most of them sit right next to their translation, even when the two words look nothing alike.",
        [
            ("English",
            [
                "dog", "cat", "house", "car", "water", "bread", "book", "tree", "sun", "moon", "city", "school",
                "friend", "money", "time", "happy", "sad", "fast", "slow", "big", "small", "to eat", "to sleep", "to run",
                "to read", "to buy", "rain", "computer", "beach", "doctor", "music", "window",
            ]),
            ("Spanish",
            [
                "perro", "gato", "casa", "coche", "agua", "pan", "libro", "árbol", "sol", "luna", "ciudad", "colegio",
                "amigo", "dinero", "tiempo", "feliz", "triste", "rápido", "lento", "grande", "pequeño", "comer", "dormir", "correr",
                "leer", "comprar", "lluvia", "ordenador", "playa", "médico", "música", "ventana",
            ]),
        ]),

        new("bookings", "Booking site messages", "Invented messages from customers of a booking website, in Spanish and English. Try searching by meaning: the best matches often share no words with the query.",
        [
            ("Missing confirmation",
            [
                "No me ha llegado el email de la reserva", "I never got the booking confirmation", "Falta la confirmación de mi reserva",
                "Where is my confirmation email?", "He pagado pero no tengo ningún justificante", "I paid but did not receive any receipt",
                "¿Me podéis reenviar el correo con los datos de la reserva?", "Can you send me the booking details again?",
                "No encuentro el mensaje de confirmación en mi bandeja", "The confirmation is not in my inbox or spam folder",
            ]),
            ("Cancellations and refunds",
            [
                "Quiero cancelar mi reserva", "I need to cancel my booking", "¿Cuándo me devolvéis el dinero?",
                "I am still waiting for my refund", "¿La cancelación es gratuita?", "Is there a fee if I cancel?",
                "Cancelé hace dos semanas y no veo el reembolso", "How long does a refund take?",
                "No puedo viajar, ¿qué opciones tengo?", "I cannot travel anymore, what can I do?",
            ]),
            ("Payment problems",
            [
                "Me han cobrado dos veces", "I was charged twice for the same room", "La tarjeta da error al pagar",
                "My card keeps getting declined", "¿Puedo pagar en el hotel en vez de online?", "Can I pay at the property instead?",
                "El precio final es más alto que el anunciado", "The total price is higher than what was shown",
                "No aparece la opción de pagar a plazos", "Which payment methods do you accept?",
            ]),
            ("Changing dates",
            [
                "Quiero cambiar las fechas de mi estancia", "Can I change the dates of my stay?", "Necesito llegar un día más tarde",
                "I would like to extend my stay by two nights", "¿Puedo añadir una noche más?", "Can I move my booking to next month?",
                "Me gustaría adelantar la salida", "Is it possible to check out a day early?",
                "¿Se puede cambiar el nombre del huésped?", "I need to change the guest name on the booking",
            ]),
            ("Room and services",
            [
                "¿La habitación tiene aire acondicionado?", "Does the room have air conditioning?", "¿Se admiten mascotas?",
                "Are pets allowed?", "¿El desayuno está incluido?", "Is breakfast included in the price?",
                "¿Hay aparcamiento cerca del hotel?", "Is there parking near the hotel?",
                "¿A qué hora se puede hacer el check-in?", "What time is check-in?",
            ]),
            ("Complaints",
            [
                "La habitación estaba sucia", "The room was not clean when we arrived", "Hacía mucho ruido por la noche",
                "It was very noisy and we could not sleep", "Las fotos no se parecen a la realidad", "The place looks nothing like the photos",
                "Nadie atendía en recepción", "There was nobody at the front desk", "La ducha no funcionaba", "The wifi did not work at all",
            ]),
        ]),

        new("dotnet", ".NET and programming", "Terms from .NET and programming in general, grouped by area.",
        [
            ("Web",
            [
                "ASP.NET Core", "minimal API", "middleware pipeline", "controller action", "Razor page", "Blazor component",
                "HTTP request", "REST endpoint", "routing", "CORS policy", "JSON response", "authentication cookie",
            ]),
            ("Data",
            [
                "Entity Framework Core", "database migration", "LINQ query", "SQL Server", "connection string", "DbContext",
                "foreign key", "stored procedure", "transaction", "index on a table", "lazy loading", "Dapper",
            ]),
            ("Testing",
            [
                "unit test", "xUnit", "mock object", "integration test", "test coverage", "assertion",
                "test fixture", "arrange act assert", "flaky test", "test runner", "snapshot testing", "end-to-end test",
            ]),
            ("Language",
            [
                "async and await", "record type", "generic method", "nullable reference types", "pattern matching", "extension method",
                "lambda expression", "interface", "dependency injection", "garbage collector", "struct vs class", "IEnumerable",
            ]),
            ("Tools",
            [
                "NuGet package", "dotnet CLI", "MSBuild", "Visual Studio", "Git branch", "GitHub Actions",
                "Docker container", "continuous integration", "code review", "debugger", "profiler", "solution file",
            ]),
        ]),
    ];
}
