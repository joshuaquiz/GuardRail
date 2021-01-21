#pragma once
#include <functional>
#include <thread>

class task
{
	std::thread* thread_;
	bool is_finished_;

public:
	void wait() const;

	static task run(std::function<void()>);

private:
	task();
	explicit task(std::thread*);
};