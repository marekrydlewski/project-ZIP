
#include "ServerZip.h"


// Global static pointer used to ensure a single instance of the class.
ServerZip* ServerZip::m_pInstance = nullptr;

/** This function is called to create an instance of the class.
    Calling the constructor publicly is not allowed. The constructor
    is private and is only called by this Instance function.
*/

ServerZip* ServerZip::getInstance()
{
    if (!m_pInstance)
        m_pInstance = new ServerZip;

    return m_pInstance;
}