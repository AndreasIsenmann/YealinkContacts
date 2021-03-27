using System;
namespace YealinkContacts
{
    public class YealinkContact
    {
        public string FullName { get; set; }

        public string PhoneOffice { get; set; }
        public string PhoneHome { get; set; }
        public string Mobile { get; set; }

        public YealinkContact()
        {
            PhoneHome = string.Empty;
            PhoneOffice = string.Empty;
            Mobile = string.Empty;
        }

        public string GetXmlString()
        {
            if (PhoneOffice == string.Empty && PhoneHome == string.Empty && Mobile == string.Empty)
                return string.Empty;

            return string.Format("<contact display_name=\"{0}\" office_number=\"{1}\" mobile_number=\"{2}\" other_number=\"{3}\" line=\"0\" ring=\"Auto\" default_photo=\"Default: default_contact_image.png\" selected_photo=\"0\" group_id_name=\"All Contacts\" />",
                FullName, PhoneOffice, Mobile, PhoneHome);
        }
    }
}
