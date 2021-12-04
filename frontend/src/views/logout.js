import { removeToken } from '../services/authService'

const Logout = ({ history }) => {
    removeToken();
    history.push("/")

    return <></>
}

export default Logout;