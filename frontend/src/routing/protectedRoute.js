import { Route, Redirect } from "react-router-dom";
import { checkAuthStatus } from "../services/authService";

const ProtectedRoute = ({ component: Component, ...restOfProps }) => {
  const isAuthenticated = checkAuthStatus();

  return (
    <Route
      {...restOfProps}
      render={(props) =>
        isAuthenticated ? <Component {...props} /> : <Redirect to="/login" />
      }
    />
  );
};

export default ProtectedRoute;
