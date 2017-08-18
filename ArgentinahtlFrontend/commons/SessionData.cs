using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using CheckArgentina.Managers;
using CheckArgentina.Models;
using System.Globalization;

namespace CheckArgentina.Commons
{
    public static class SessionData
    {
        private static HttpSessionState Session
        {
            get { return Context.Session; }
        }

        private static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        public static Credential UserCredential
        {
            get
            {
                var reservation = Session["UserCredential"];

                if (reservation == null)
                    reservation = new Credential()
                    {
                        Username = string.Empty,
                        Password = string.Empty,
                        Language = "es"
                    };

                return (Credential)reservation;
            }
            set
            {
                Session["UserCredential"] = value;
            }
        }

        public static UserModel User
        {
            get
            {
                var user = Session["User"];

                if (user == null)
                    user = new UserModel()
                    {
                        UserId = Guid.Empty,
                        UsesPaymentInterface = true
                    };

                return (UserModel)user;
            }
            set
            {
                Session["User"] = value;
            }
        }

        public static CheckArgentina.Managers.SearchType SearchType
        {
            get
            {
                var type = Session["SearchType"];

                if (type == null)
                    return CheckArgentina.Managers.SearchType.Undefined;

                return (CheckArgentina.Managers.SearchType)type;
            }
            set
            {
                Session["SearchType"] = value;
            }
        }

        public static ReservationModel Reservation
        {
            get 
            {
                var reservation = Session["Reservation"];

                if (reservation == null)
                    reservation = new ReservationModel() 
                    {
                        ReservationStatus = ReservationStatus.Error
                    };

                return (ReservationModel)reservation;
            }
            set 
            {
                Session["Reservation"] = value;
            }
        }

        public static Dictionary<string, string> Countries
        {
            get
            {
                if (Session["Countries"] == null)
                {
                    var countries = new Dictionary<string, string> { 
                        {"AF", "Afghanistan"},
                        {"AX", "Åland Islands"},
                        {"AL", "Albania"},
                        {"DZ", "Algeria"},
                        {"AS", "American Samoa"},
                        {"AD", "Andorra"},
                        {"AO", "Angola"},
                        {"AI", "Anguilla"},
                        {"AQ", "Antarctica"},
                        {"AG", "Antigua And Barbuda"},
                        {"AR", "Argentina"},
                        {"AM", "Armenia"},
                        {"AW", "Aruba"},
                        {"AU", "Australia"},
                        {"AT", "Austria"},
                        {"AZ", "Azerbaijan"},
                        {"BS", "Bahamas"},
                        {"BH", "Bahrain"},
                        {"BD", "Bangladesh"},
                        {"BB", "Barbados"},
                        {"BY", "Belarus"},
                        {"BE", "Belgium"},
                        {"BZ", "Belize"},
                        {"BJ", "Benin"},
                        {"BM", "Bermuda"},
                        {"BT", "Bhutan"},
                        {"BO", "Bolivia, Plurinational State Of"},
                        {"BQ", "Bonaire, Sint Eustatius And Saba"},
                        {"BA", "Bosnia And Herzegovina"},
                        {"BW", "Botswana"},
                        {"BV", "Bouvet Island"},
                        {"BR", "Brazil"},
                        {"IO", "British Indian Ocean Territory"},
                        {"BN", "Brunei Darussalam"},
                        {"BG", "Bulgaria"},
                        {"BF", "Burkina Faso"},
                        {"BI", "Burundi"},
                        {"KH", "Cambodia"},
                        {"CM", "Cameroon"},
                        {"CA", "Canada"},
                        {"CV", "Cape Verde"},
                        {"KY", "Cayman Islands"},
                        {"CF", "Central African Republic"},
                        {"TD", "Chad"},
                        {"CL", "Chile"},
                        {"CN", "China"},
                        {"CX", "Christmas Island"},
                        {"CC", "Cocos (Keeling) Islands"},
                        {"CO", "Colombia"},
                        {"KM", "Comoros"},
                        {"CG", "Congo"},
                        {"CD", "Congo, The Democratic Republic Of The"},
                        {"CK", "Cook Islands"},
                        {"CR", "Costa Rica"},
                        {"CI", "Côte D'Ivoire"},
                        {"HR", "Croatia"},
                        {"CU", "Cuba"},
                        {"CW", "Curaçao"},
                        {"CY", "Cyprus"},
                        {"CZ", "Czech Republic"},
                        {"DK", "Denmark"},
                        {"DJ", "Djibouti"},
                        {"DM", "Dominica"},
                        {"DO", "Dominican Republic"},
                        {"EC", "Ecuador"},
                        {"EG", "Egypt"},
                        {"SV", "El Salvador"},
                        {"GQ", "Equatorial Guinea"},
                        {"ER", "Eritrea"},
                        {"EE", "Estonia"},
                        {"ET", "Ethiopia"},
                        {"FK", "Falkland Islands (Malvinas)"},
                        {"FO", "Faroe Islands"},
                        {"FJ", "Fiji"},
                        {"FI", "Finland"},
                        {"FR", "France"},
                        {"GF", "French Guiana"},
                        {"PF", "French Polynesia"},
                        {"TF", "French Southern Territories"},
                        {"GA", "Gabon"},
                        {"GM", "Gambia"},
                        {"GE", "Georgia"},
                        {"DE", "Germany"},
                        {"GH", "Ghana"},
                        {"GI", "Gibraltar"},
                        {"GR", "Greece"},
                        {"GL", "Greenland"},
                        {"GD", "Grenada"},
                        {"GP", "Guadeloupe"},
                        {"GU", "Guam"},
                        {"GT", "Guatemala"},
                        {"GG", "Guernsey"},
                        {"GN", "Guinea"},
                        {"GW", "Guinea-Bissau"},
                        {"GY", "Guyana"},
                        {"HT", "Haiti"},
                        {"HM", "Heard Island And Mcdonald Islands"},
                        {"VA", "Holy See (Vatican City State)"},
                        {"HN", "Honduras"},
                        {"HK", "Hong Kong"},
                        {"HU", "Hungary"},
                        {"IS", "Iceland"},
                        {"IN", "India"},
                        {"ID", "Indonesia"},
                        {"IR", "Iran, Islamic Republic Of"},
                        {"IQ", "Iraq"},
                        {"IE", "Ireland"},
                        {"IM", "Isle Of Man"},
                        {"IL", "Israel"},
                        {"IT", "Italy"},
                        {"JM", "Jamaica"},
                        {"JP", "Japan"},
                        {"JE", "Jersey"},
                        {"JO", "Jordan"},
                        {"KZ", "Kazakhstan"},
                        {"KE", "Kenya"},
                        {"KI", "Kiribati"},
                        {"KP", "Korea, Democratic People'S Republic Of"},
                        {"KR", "Korea, Republic Of"},
                        {"KW", "Kuwait"},
                        {"KG", "Kyrgyzstan"},
                        {"LA", "Lao People'S Democratic Republic"},
                        {"LV", "Latvia"},
                        {"LB", "Lebanon"},
                        {"LS", "Lesotho"},
                        {"LR", "Liberia"},
                        {"LY", "Libya"},
                        {"LI", "Liechtenstein"},
                        {"LT", "Lithuania"},
                        {"LU", "Luxembourg"},
                        {"MO", "Macao"},
                        {"MK", "Macedonia, The Former Yugoslav Republic Of"},
                        {"MG", "Madagascar"},
                        {"MW", "Malawi"},
                        {"MY", "Malaysia"},
                        {"MV", "Maldives"},
                        {"ML", "Mali"},
                        {"MT", "Malta"},
                        {"MH", "Marshall Islands"},
                        {"MQ", "Martinique"},
                        {"MR", "Mauritania"},
                        {"MU", "Mauritius"},
                        {"YT", "Mayotte"},
                        {"MX", "Mexico"},
                        {"FM", "Micronesia, Federated States Of"},
                        {"MD", "Moldova, Republic Of"},
                        {"MC", "Monaco"},
                        {"MN", "Mongolia"},
                        {"ME", "Montenegro"},
                        {"MS", "Montserrat"},
                        {"MA", "Morocco"},
                        {"MZ", "Mozambique"},
                        {"MM", "Myanmar"},
                        {"NA", "Namibia"},
                        {"NR", "Nauru"},
                        {"NP", "Nepal"},
                        {"NL", "Netherlands"},
                        {"NC", "New Caledonia"},
                        {"NZ", "New Zealand"},
                        {"NI", "Nicaragua"},
                        {"NE", "Niger"},
                        {"NG", "Nigeria"},
                        {"NU", "Niue"},
                        {"NF", "Norfolk Island"},
                        {"MP", "Northern Mariana Islands"},
                        {"NO", "Norway"},
                        {"OM", "Oman"},
                        {"PK", "Pakistan"},
                        {"PW", "Palau"},
                        {"PS", "Palestinian Territory, Occupied"},
                        {"PA", "Panama"},
                        {"PG", "Papua New Guinea"},
                        {"PY", "Paraguay"},
                        {"PE", "Peru"},
                        {"PH", "Philippines"},
                        {"PN", "Pitcairn"},
                        {"PL", "Poland"},
                        {"PT", "Portugal"},
                        {"PR", "Puerto Rico"},
                        {"QA", "Qatar"},
                        {"RE", "Réunion"},
                        {"RO", "Romania"},
                        {"RU", "Russian Federation"},
                        {"RW", "Rwanda"},
                        {"BL", "Saint Barthélemy"},
                        {"SH", "Saint Helena, Ascension And Tristan Da Cunha"},
                        {"KN", "Saint Kitts And Nevis"},
                        {"LC", "Saint Lucia"},
                        {"MF", "Saint Martin (French Part)"},
                        {"PM", "Saint Pierre And Miquelon"},
                        {"VC", "Saint Vincent And The Grenadines"},
                        {"WS", "Samoa"},
                        {"SM", "San Marino"},
                        {"ST", "Sao Tome And Principe"},
                        {"SA", "Saudi Arabia"},
                        {"SN", "Senegal"},
                        {"RS", "Serbia"},
                        {"SC", "Seychelles"},
                        {"SL", "Sierra Leone"},
                        {"SG", "Singapore"},
                        {"SX", "Sint Maarten (Dutch Part)"},
                        {"SK", "Slovakia"},
                        {"SI", "Slovenia"},
                        {"SB", "Solomon Islands"},
                        {"SO", "Somalia"},
                        {"ZA", "South Africa"},
                        {"GS", "South Georgia And The South Sandwich Islands"},
                        {"SS", "South Sudan"},
                        {"ES", "Spain"},
                        {"LK", "Sri Lanka"},
                        {"SD", "Sudan"},
                        {"SR", "Suriname"},
                        {"SJ", "Svalbard And Jan Mayen"},
                        {"SZ", "Swaziland"},
                        {"SE", "Sweden"},
                        {"CH", "Switzerland"},
                        {"SY", "Syrian Arab Republic"},
                        {"TW", "Taiwan, Province Of China"},
                        {"TJ", "Tajikistan"},
                        {"TZ", "Tanzania, United Republic Of"},
                        {"TH", "Thailand"},
                        {"TL", "Timor-Leste"},
                        {"TG", "Togo"},
                        {"TK", "Tokelau"},
                        {"TO", "Tonga"},
                        {"TT", "Trinidad And Tobago"},
                        {"TN", "Tunisia"},
                        {"TR", "Turkey"},
                        {"TM", "Turkmenistan"},
                        {"TC", "Turks And Caicos Islands"},
                        {"TV", "Tuvalu"},
                        {"UG", "Uganda"},
                        {"UA", "Ukraine"},
                        {"AE", "United Arab Emirates"},
                        {"GB", "United Kingdom"},
                        {"US", "United States"},
                        {"UM", "United States Minor Outlying Islands"},
                        {"UY", "Uruguay"},
                        {"UZ", "Uzbekistan"},
                        {"VU", "Vanuatu"},
                        {"VE", "Venezuela, Bolivarian Republic Of"},
                        {"VN", "Viet Nam"},
                        {"VG", "Virgin Islands, British"},
                        {"VI", "Virgin Islands, U.S."},
                        {"WF", "Wallis And Futuna"},
                        {"EH", "Western Sahara"},
                        {"YE", "Yemen"},
                        {"ZM", "Zambia"},
                        {"ZW", "Zimbabwe"}
                    };

                    Session["Countries"] = countries;
                }

                return (Dictionary<string, string>)Session["Countries"];
            }
        }

        public static Dictionary<string, string> PaymentMethods
        {
            get
            {
                var paymentMethods = Session["PaymentMethods" + SearchType];

                if (paymentMethods == null)
                    paymentMethods = new Manager(SearchType).GetPaymentMethods();

                return (Dictionary<string, string>)paymentMethods;
            }
        }

        public static void Clear()
        {
            Session.Clear();
        }
    }
}