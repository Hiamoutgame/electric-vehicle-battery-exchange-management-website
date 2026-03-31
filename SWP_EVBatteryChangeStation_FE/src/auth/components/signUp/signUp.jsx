import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import authService from "@/api/authService";
import Button from "../../../components/button";
import "./signUp.css";

const SignUp = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (password !== confirm) {
      toast.error("Mat khau khong trung khop.");
      return;
    }

    try {
      await authService.register(email, password);
      localStorage.setItem("pendingVerificationEmail", email);
      toast.success("Da tao OTP. Kiem tra console/log backend de lay ma xac thuc.");
      navigate("/verifyOtp", { state: { email } });
    } catch (error) {
      console.error("Sign up failed:", error);
      toast.error(authService.getErrorMessage(error, "Dang ky that bai."));
    }
  };

  return (
    <div className="auth-signup-container">
      <div className="auth-signup-header">
        <h1>Sign Up</h1>
        <p>Create a new account</p>
      </div>

      <form className="auth-form-group" onSubmit={handleSubmit}>
        <div className="auth-form-block">
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="Enter email address"
            required
          />
        </div>

        <div className="auth-form-block">
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="Password"
            required
            minLength="6"
          />
        </div>

        <div className="auth-form-block">
          <label>Confirm Password</label>
          <input
            type="password"
            value={confirm}
            onChange={(event) => setConfirm(event.target.value)}
            placeholder="Confirm password"
            required
          />
        </div>

        <div className="auth-btn-group">
          <Button type="submit">Sign Up</Button>
        </div>

        <div className="auth-footer">
          <p>
            Already have an account? <Link to="/login">Sign In</Link>
          </p>
        </div>
      </form>
    </div>
  );
};

export default SignUp;
