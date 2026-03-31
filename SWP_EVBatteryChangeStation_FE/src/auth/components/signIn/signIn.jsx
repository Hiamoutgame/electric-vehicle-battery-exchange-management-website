import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Button from "../../../components/button";
import { notifySuccess, notifyError } from "@/components/notification/notification";
import authService from "@/api/authService";
import tokenUtils from "@/utils/tokenUtils";
import roleService from "@/api/roleService";
import "./signIn.css";

const getRedirectPath = async (user) => {
  if (!user) return "/userPage";
  return roleService.getRedirectPathByRole(user.roleName || user.role, user.roleId);
};

const SignIn = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const checkExistingLogin = async () => {
      if (tokenUtils.isLoggedIn()) {
        const userData = await tokenUtils.autoLogin();
        if (userData) {
          navigate(await getRedirectPath(userData));
        }
      }
    };

    checkExistingLogin();
  }, [navigate]);

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const loginData = await authService.login(email, password);
      const token = loginData?.accessToken;

      if (!token) {
        notifyError("Đăng nhập thất bại.");
        return;
      }

      localStorage.setItem("token", token);
      const userProfile = await tokenUtils.processLoginToken(token);

      if (!userProfile) {
        notifyError("Không thể tải hồ sơ người dùng.");
        return;
      }

      notifySuccess(`Xin chào ${userProfile.fullName || userProfile.accountName}!`);
      navigate(await getRedirectPath(userProfile));
    } catch (error) {
      notifyError(
        authService.getErrorMessage(
          error,
          "Sai tài khoản hoặc mật khẩu."
        )
      );
    }
  };

  return (
    <div className="signIn">
      <div className="header-signIn">
        <h1>WELCOME BACK!</h1>
        <p>Please login to your account</p>
      </div>

      <form className="signIn-Group" onSubmit={handleSubmit}>
        <div className="signIn-block">
          <label>Email Address:</label>
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="Email Address"
            required
          />
        </div>

        <div className="signIn-block">
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="Password"
            required
          />
        </div>

        <div className="butt">
          <Button type="submit">Sign in</Button>
        </div>

        <div className="footer-text">
          <p>
            Don't have an account yet?
            <Link to="/signup"> Create an account </Link>
          </p>
          <p style={{ marginTop: 8 }}>
            <Link to="/forgot-password">Forgot password?</Link>
          </p>
        </div>
      </form>
    </div>
  );
};

export default SignIn;
