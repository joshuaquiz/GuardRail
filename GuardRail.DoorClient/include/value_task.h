#pragma once
#include <thread>

template <typename T>
class value_task
{
	std::thread* thread_;
	T result_;
	bool is_finished_;
public:
	void wait() const;
	T get_result();

	static value_task run(T(*a)());

private:
	value_task();
	explicit value_task(std::thread*);
};